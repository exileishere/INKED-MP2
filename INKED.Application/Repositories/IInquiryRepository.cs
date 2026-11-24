using INKED.Domain.Entities.dbo;

namespace INKED.Application.Repositories;

public interface IInquiryRepository
{
    Task<IReadOnlyList<Inquiry>> GetAllAsync(CancellationToken ct = default);
    Task<Inquiry?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Inquiry> AddAsync(Inquiry entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Inquiry entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    // Foreign-key lookups and dependants (read-only checks on the shared DbContext)
    Task<bool> CustomerExistsAsync(int customerId, CancellationToken ct = default);
    Task<bool> UserExistsAsync(int userId, CancellationToken ct = default);
    Task<bool> HasQuotationsAsync(int inquiryId, CancellationToken ct = default);
    Task<bool> HasFeasibilityChecksAsync(int inquiryId, CancellationToken ct = default);
}
