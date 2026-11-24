using INKED.Application.Models.DTO.Quotations;

namespace INKED.Application.Interfaces;

public interface IQuotationService
{
    Task<IReadOnlyList<QuotationResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<QuotationResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<QuotationResponseDto> CreateAsync(CreateQuotationDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateQuotationDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
