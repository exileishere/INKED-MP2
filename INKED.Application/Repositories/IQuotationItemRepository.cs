using INKED.Domain.Entities.dbo;

namespace INKED.Application.Repositories;

public interface IQuotationItemRepository
{
    Task<IReadOnlyList<QuotationItem>> GetAllAsync(CancellationToken ct = default);
    Task<QuotationItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<QuotationItem> AddAsync(QuotationItem entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(QuotationItem entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<bool> QuotationExistsAsync(int quotationId, CancellationToken ct = default);
    Task<bool> ProductExistsAsync(int productId, CancellationToken ct = default);
    Task<bool> HasSizeChartsAsync(int quotationItemId, CancellationToken ct = default);
}
