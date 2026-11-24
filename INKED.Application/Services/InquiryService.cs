using INKED.Application.Helpers;
using INKED.Application.Exceptions;
using INKED.Application.Models.DTO.Inquiries;
using INKED.Application.Repositories;
using INKED.Application.Interfaces;
using INKED.Domain.Entities.dbo;

namespace INKED.Application.Services;

public class InquiryService : IInquiryService
{
    // Mirror CK_Inquiries_Channel and CK_Inquiries_Outcome in the MP1 script.
    private static readonly string[] Channels = { "Inquiry Form", "Facebook", "Instagram", "Shopee", "TikTok", "Other" };
    private static readonly string[] Outcomes = { "Open", "Quoted", "Converted", "Not Pursued" };

    private readonly IInquiryRepository _repository;

    public InquiryService(IInquiryRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<InquiryResponseDto>> GetAllAsync(CancellationToken ct = default)
        => (await _repository.GetAllAsync(ct)).Select(ToResponse).ToList();

    public async Task<InquiryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<InquiryResponseDto> CreateAsync(CreateInquiryDto dto, CancellationToken ct = default)
    {
        if (!await _repository.CustomerExistsAsync(dto.CustomerId, ct))
            throw new BadRequestException($"Customer {dto.CustomerId} does not exist.");

        if (dto.HandledByUserId is int userId && !await _repository.UserExistsAsync(userId, ct))
            throw new BadRequestException($"User account {userId} does not exist.");

        var channel = ValidationRules.NormalizeChoice(dto.InquiryChannel ?? "Inquiry Form", "InquiryChannel", Channels);
        var outcome = ValidationRules.NormalizeChoice(dto.Outcome ?? "Open", "Outcome", Outcomes);

        var received = dto.DateReceived is { } r ? ValidationRules.ToSecond(r) : ValidationRules.NowToSecond();
        var firstResponse = dto.FirstResponseAt is { } f ? ValidationRules.ToSecond(f) : (DateTime?)null;
        EnsureFirstResponse(received, firstResponse);

        var entity = new Inquiry
        {
            CustomerId = dto.CustomerId,
            HandledByUserId = dto.HandledByUserId,
            DateReceived = received,
            InquiryChannel = channel,
            InquiryDetails = ValidationRules.NullIfBlank(dto.InquiryDetails),
            NeedsDesignHelp = dto.NeedsDesignHelp,
            FirstResponseAt = firstResponse,
            Outcome = outcome
        };

        await _repository.AddAsync(entity, ct);
        return ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateInquiryDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        if (dto.HandledByUserId is int userId && !await _repository.UserExistsAsync(userId, ct))
            throw new BadRequestException($"User account {userId} does not exist.");

        var channel = ValidationRules.NormalizeChoice(dto.InquiryChannel, "InquiryChannel", Channels);
        var outcome = ValidationRules.NormalizeChoice(dto.Outcome, "Outcome", Outcomes);

        var firstResponse = dto.FirstResponseAt is { } f ? ValidationRules.ToSecond(f) : (DateTime?)null;
        EnsureFirstResponse(entity.DateReceived, firstResponse);

        entity.HandledByUserId = dto.HandledByUserId;
        entity.InquiryChannel = channel;
        entity.InquiryDetails = ValidationRules.NullIfBlank(dto.InquiryDetails);
        entity.NeedsDesignHelp = dto.NeedsDesignHelp;
        entity.FirstResponseAt = firstResponse;
        entity.Outcome = outcome;

        return await _repository.UpdateAsync(entity, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (await _repository.GetByIdAsync(id, ct) is null) return false;

        if (await _repository.HasQuotationsAsync(id, ct))
            throw new ConflictException($"Inquiry {id} cannot be deleted because it has quotations.");

        if (await _repository.HasFeasibilityChecksAsync(id, ct))
            throw new ConflictException($"Inquiry {id} cannot be deleted because it has feasibility checks.");

        return await _repository.DeleteAsync(id, ct);
    }

    // CK_Inquiries_FirstResponse
    private static void EnsureFirstResponse(DateTime received, DateTime? firstResponse)
    {
        if (firstResponse is { } f && f < received)
            throw new BadRequestException("FirstResponseAt cannot be earlier than DateReceived.");
    }

    private static InquiryResponseDto ToResponse(Inquiry e) => new()
    {
        InquiryId = e.InquiryId,
        CustomerId = e.CustomerId,
        HandledByUserId = e.HandledByUserId,
        DateReceived = e.DateReceived,
        InquiryChannel = e.InquiryChannel,
        InquiryDetails = e.InquiryDetails,
        NeedsDesignHelp = e.NeedsDesignHelp,
        FirstResponseAt = e.FirstResponseAt,
        Outcome = e.Outcome
    };
}
