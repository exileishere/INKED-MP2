using INKED.Application.DTOs;

namespace INKED.Application.Interfaces;

public interface ISampleApprovalService
{
    Task<IReadOnlyList<SampleApprovalResponseDto>> GetAllAsync();
    Task<SampleApprovalResponseDto> GetByIdAsync(int id);
    Task<SampleApprovalResponseDto> CreateAsync(CreateSampleApprovalDto dto);
    Task<SampleApprovalResponseDto> UpdateAsync(int id, UpdateSampleApprovalDto dto);
    Task DeleteAsync(int id);
}
