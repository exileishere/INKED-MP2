using INKED.Domain.Entities.dbo;

namespace INKED.Application.Repositories;

public interface IQuotationRepository
{
    Task<IReadOnlyList<Quotation>> GetAllAsync(CancellationToken ct = default);
    Task<Quotation?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Quotation> AddAsync(Quotation entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Quotation entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<bool> InquiryExistsAsync(int inquiryId, CancellationToken ct = default);
    Task<bool> UserExistsAsync(int userId, CancellationToken ct = default);
    Task<bool> HasItemsAsync(int quotationId, CancellationToken ct = default);
    Task<bool> HasOrderAsync(int quotationId, CancellationToken ct = default);
}
