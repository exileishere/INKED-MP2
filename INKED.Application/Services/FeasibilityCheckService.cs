using INKED.Application.Helpers;
using INKED.Application.Exceptions;
using INKED.Application.Models.DTO.FeasibilityChecks;
using INKED.Application.Repositories;
using INKED.Application.Interfaces;
using INKED.Domain.Entities.dbo;

namespace INKED.Application.Services;

public class FeasibilityCheckService : IFeasibilityCheckService
{
    // Mirrors CK_FeasibilityChecks_Result in the MP1 script.
    private static readonly string[] Results = { "Pending", "Feasible", "Not Feasible" };

    private readonly IFeasibilityCheckRepository _repository;

    public FeasibilityCheckService(IFeasibilityCheckRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<FeasibilityCheckResponseDto>> GetAllAsync(CancellationToken ct = default)
        => (await _repository.GetAllAsync(ct)).Select(ToResponse).ToList();

    public async Task<FeasibilityCheckResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<FeasibilityCheckResponseDto> CreateAsync(CreateFeasibilityCheckDto dto, CancellationToken ct = default)
    {
        if (!await _repository.InquiryExistsAsync(dto.InquiryId, ct))
            throw new BadRequestException($"Inquiry {dto.InquiryId} does not exist.");

        if (!await _repository.TeamExistsAsync(dto.TeamId, ct))
            throw new BadRequestException($"Production team {dto.TeamId} does not exist.");

        var result = ValidationRules.NormalizeChoice(dto.Result ?? "Pending", "Result", Results);

        var requested = dto.RequestedAt is { } r ? ValidationRules.ToSecond(r) : ValidationRules.NowToSecond();
        var responded = dto.RespondedAt is { } p ? ValidationRules.ToSecond(p) : (DateTime?)null;
        EnsureDates(requested, responded);

        var entity = new FeasibilityCheck
        {
            InquiryId = dto.InquiryId,
            TeamId = dto.TeamId,
            Result = result,
            Remarks = ValidationRules.NullIfBlank(dto.Remarks),
            RequestedAt = requested,
            RespondedAt = responded
        };

        await _repository.AddAsync(entity, ct);
        return ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateFeasibilityCheckDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        if (!await _repository.TeamExistsAsync(dto.TeamId, ct))
            throw new BadRequestException($"Production team {dto.TeamId} does not exist.");

        var result = ValidationRules.NormalizeChoice(dto.Result, "Result", Results);

        var responded = dto.RespondedAt is { } p ? ValidationRules.ToSecond(p) : (DateTime?)null;
        EnsureDates(entity.RequestedAt, responded);

        entity.TeamId = dto.TeamId;
        entity.Result = result;
        entity.Remarks = ValidationRules.NullIfBlank(dto.Remarks);
        entity.RespondedAt = responded;

        return await _repository.UpdateAsync(entity, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (await _repository.GetByIdAsync(id, ct) is null) return false;
        return await _repository.DeleteAsync(id, ct);
    }

    // CK_FeasibilityChecks_Dates
    private static void EnsureDates(DateTime requested, DateTime? responded)
    {
        if (responded is { } r && r < requested)
            throw new BadRequestException("RespondedAt cannot be earlier than RequestedAt.");
    }

    private static FeasibilityCheckResponseDto ToResponse(FeasibilityCheck e) => new()
    {
        FeasibilityId = e.FeasibilityId,
        InquiryId = e.InquiryId,
        TeamId = e.TeamId,
        Result = e.Result,
        Remarks = e.Remarks,
        RequestedAt = e.RequestedAt,
        RespondedAt = e.RespondedAt
    };
}
