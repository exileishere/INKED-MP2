using INKED.Application.Models.DTO.QuotationItems;

namespace INKED.Application.Interfaces;

public interface IQuotationItemService
{
    Task<IReadOnlyList<QuotationItemResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<QuotationItemResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<QuotationItemResponseDto> CreateAsync(CreateQuotationItemDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateQuotationItemDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
