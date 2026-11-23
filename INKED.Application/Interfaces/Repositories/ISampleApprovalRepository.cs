using INKED.Application.DTOs;

namespace INKED.Application.Interfaces;

public interface ISampleApprovalRepository
{
    Task<IReadOnlyList<SampleApprovalResponseDto>> GetAllAsync();
    Task<SampleApprovalResponseDto?> GetByIdAsync(int id);
    Task<SampleApprovalResponseDto> AddAsync(SampleApprovalResponseDto sample);   // SampleApprovalID is ignored
    Task UpdateAsync(SampleApprovalResponseDto sample);
    Task DeleteAsync(int id);

    Task<bool> OrderExistsAsync(int orderId);
    /// <summary>UNIQUE (OrderID, RevisionNo).</summary>
    Task<bool> RevisionExistsAsync(int orderId, byte revisionNo);
    /// <summary>Highest RevisionNo of the order + 1 (1 if none).</summary>
    Task<int> GetNextRevisionNoAsync(int orderId);
}
