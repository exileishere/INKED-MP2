using INKED.Application.Helpers;
using INKED.Application.Exceptions;
using INKED.Application.Models.DTO.Quotations;
using INKED.Application.Repositories;
using INKED.Application.Interfaces;
using INKED.Domain.Entities.dbo;

namespace INKED.Application.Services;

public class QuotationService : IQuotationService
{
    // Mirrors CK_Quotations_Status in the MP1 script.
    private static readonly string[] Statuses = { "Draft", "Sent", "Accepted", "Rejected", "Expired" };

    private readonly IQuotationRepository _repository;

    public QuotationService(IQuotationRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<QuotationResponseDto>> GetAllAsync(CancellationToken ct = default)
        => (await _repository.GetAllAsync(ct)).Select(ToResponse).ToList();

    public async Task<QuotationResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<QuotationResponseDto> CreateAsync(CreateQuotationDto dto, CancellationToken ct = default)
    {
        if (!await _repository.InquiryExistsAsync(dto.InquiryId, ct))
            throw new BadRequestException($"Inquiry {dto.InquiryId} does not exist.");

        if (!await _repository.UserExistsAsync(dto.PreparedByUserId, ct))
            throw new BadRequestException($"User account {dto.PreparedByUserId} does not exist.");

        var status = ValidationRules.NormalizeChoice(dto.Status ?? "Draft", "Status", Statuses);
        ValidationRules.EnsureMoney(dto.TotalAmount, "TotalAmount", precision: 12);

        var entity = new Quotation
        {
            InquiryId = dto.InquiryId,
            PreparedByUserId = dto.PreparedByUserId,
            DateIssued = dto.DateIssued is { } d ? ValidationRules.ToSecond(d) : ValidationRules.NowToSecond(),
            TotalAmount = dto.TotalAmount,
            Status = status
        };

        await _repository.AddAsync(entity, ct);
        return ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuotationDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        if (!await _repository.UserExistsAsync(dto.PreparedByUserId, ct))
            throw new BadRequestException($"User account {dto.PreparedByUserId} does not exist.");

        var status = ValidationRules.NormalizeChoice(dto.Status, "Status", Statuses);
        ValidationRules.EnsureMoney(dto.TotalAmount, "TotalAmount", precision: 12);

        entity.PreparedByUserId = dto.PreparedByUserId;
        entity.TotalAmount = dto.TotalAmount;
        entity.Status = status;

        return await _repository.UpdateAsync(entity, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (await _repository.GetByIdAsync(id, ct) is null) return false;

        if (await _repository.HasOrderAsync(id, ct))
            throw new ConflictException($"Quotation {id} cannot be deleted because an order was created from it.");

        if (await _repository.HasItemsAsync(id, ct))
            throw new ConflictException($"Quotation {id} cannot be deleted because it still has quotation items.");

        return await _repository.DeleteAsync(id, ct);
    }

    private static QuotationResponseDto ToResponse(Quotation e) => new()
    {
        QuotationId = e.QuotationId,
        InquiryId = e.InquiryId,
        PreparedByUserId = e.PreparedByUserId,
        DateIssued = e.DateIssued,
        TotalAmount = e.TotalAmount,
        Status = e.Status
    };
}
