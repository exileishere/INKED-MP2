using INKED.Application.Models.DTO.Inquiries;

namespace INKED.Application.Interfaces;

public interface IInquiryService
{
    Task<IReadOnlyList<InquiryResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<InquiryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<InquiryResponseDto> CreateAsync(CreateInquiryDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateInquiryDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
