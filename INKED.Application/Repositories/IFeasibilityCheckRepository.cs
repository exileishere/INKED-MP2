using INKED.Domain.Entities.dbo;

namespace INKED.Application.Repositories;

public interface IFeasibilityCheckRepository
{
    Task<IReadOnlyList<FeasibilityCheck>> GetAllAsync(CancellationToken ct = default);
    Task<FeasibilityCheck?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<FeasibilityCheck> AddAsync(FeasibilityCheck entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(FeasibilityCheck entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<bool> InquiryExistsAsync(int inquiryId, CancellationToken ct = default);
    Task<bool> TeamExistsAsync(int teamId, CancellationToken ct = default);
}
