namespace INKED.Application.SizeCharts;

/// <summary>Application operations for dbo.SizeCharts.</summary>
public interface ISizeChartService
{
    Task<List<SizeChartResponseDto>> GetAllAsync();
    Task<SizeChartResponseDto?> GetByIdAsync(int id);
    Task<SizeChartResponseDto> CreateAsync(CreateSizeChartDto dto);
    Task<bool> UpdateAsync(int id, UpdateSizeChartDto dto);
    Task<bool> DeleteAsync(int id);
}
