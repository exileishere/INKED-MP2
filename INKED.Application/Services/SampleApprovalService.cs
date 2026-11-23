using INKED.Application.Common.Exceptions;
using INKED.Application.DTOs;
using INKED.Application.Interfaces;

namespace INKED.Application.Services;

public class SampleApprovalService : ISampleApprovalService
{
    // Must match CK_SampleApprovals_Status in the MP1 script.
    private static readonly string[] AllowedStatuses = { "Pending", "Approved", "Revision Requested" };

    private readonly ISampleApprovalRepository _samples;

    public SampleApprovalService(ISampleApprovalRepository samples) => _samples = samples;

    public async Task<IReadOnlyList<SampleApprovalResponseDto>> GetAllAsync()
        => await _samples.GetAllAsync();

    public async Task<SampleApprovalResponseDto> GetByIdAsync(int id)
        => await GetOrThrowAsync(id);

    public async Task<SampleApprovalResponseDto> CreateAsync(CreateSampleApprovalDto dto)
    {
        if (!await _samples.OrderExistsAsync(dto.OrderID))
            throw new BadRequestException($"OrderID {dto.OrderID} does not exist in Orders.");

        var status = NormalizeStatus(dto.ApprovalStatus ?? "Pending");

        int revision;
        if (dto.RevisionNo.HasValue)
        {
            revision = dto.RevisionNo.Value;
            if (await _samples.RevisionExistsAsync(dto.OrderID, (byte)revision))
                throw new ConflictException(
                    $"Order {dto.OrderID} already has a sample with revision number {revision}.");
        }
        else
        {
            revision = await _samples.GetNextRevisionNoAsync(dto.OrderID);
            if (revision > byte.MaxValue)
                throw new ConflictException("The maximum number of sample revisions (255) was reached for this order.");
        }

        var dateSent = dto.DateSent ?? DateTime.Now;

        var data = new SampleApprovalResponseDto
        {
            OrderID = dto.OrderID,
            RevisionNo = (byte)revision,
            SamplePhotoPath = Optional(dto.SamplePhotoPath),
            DateSent = dateSent,
            ApprovalStatus = status,
            DateResponded = ResolveResponseDate(status, dto.DateResponded, dateSent),
            CustomerRemarks = Optional(dto.CustomerRemarks)
        };

        return await _samples.AddAsync(data);
    }

    public async Task<SampleApprovalResponseDto> UpdateAsync(int id, UpdateSampleApprovalDto dto)
    {
        var sample = await GetOrThrowAsync(id);
        var status = NormalizeStatus(dto.ApprovalStatus);

        sample.SamplePhotoPath = Optional(dto.SamplePhotoPath);
        sample.ApprovalStatus = status;
        sample.DateResponded = ResolveResponseDate(status, dto.DateResponded, sample.DateSent);
        sample.CustomerRemarks = Optional(dto.CustomerRemarks);

        await _samples.UpdateAsync(sample);
        return sample;
    }

    public async Task DeleteAsync(int id)
    {
        await GetOrThrowAsync(id);
        await _samples.DeleteAsync(id);
    }

    // ---- helpers ----
    private async Task<SampleApprovalResponseDto> GetOrThrowAsync(int id)
        => await _samples.GetByIdAsync(id)
           ?? throw new NotFoundException($"Sample approval with ID {id} was not found.");

    private static string NormalizeStatus(string? input)
    {
        var match = AllowedStatuses.FirstOrDefault(
            s => s.Equals(input?.Trim(), StringComparison.OrdinalIgnoreCase));
        return match ?? throw new BadRequestException(
            $"ApprovalStatus must be one of: {string.Join(", ", AllowedStatuses)}.");
    }

    /// <summary>
    /// Mirrors CK_SampleApprovals_Responded and CK_SampleApprovals_Dates:
    /// Pending => DateResponded must be empty; otherwise it is required and cannot be before DateSent.
    /// </summary>
    private static DateTime? ResolveResponseDate(string status, DateTime? dateResponded, DateTime dateSent)
    {
        if (status == "Pending")
        {
            if (dateResponded.HasValue)
                throw new BadRequestException("DateResponded must be empty while ApprovalStatus is Pending.");
            return null;
        }

        var responded = dateResponded ?? DateTime.Now;
        if (responded < dateSent)
            throw new BadRequestException("DateResponded cannot be earlier than DateSent.");
        return responded;
    }

    private static string? Optional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
