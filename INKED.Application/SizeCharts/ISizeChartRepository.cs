namespace INKED.Application.SizeCharts;

/// <summary>
/// Data-access operations for dbo.SizeCharts. Implemented in INKED.Infrastructure with EF Core.
/// It works with DTOs so the Application layer does not depend on the scaffolded entity classes.
/// </summary>
public interface ISizeChartRepository
{
    Task<List<SizeChartResponseDto>> GetAllAsync();
    Task<SizeChartResponseDto?> GetByIdAsync(int id);
    Task<SizeChartResponseDto> AddAsync(CreateSizeChartDto dto);
    Task<bool> UpdateAsync(int id, UpdateSizeChartDto dto);
    Task<bool> DeleteAsync(int id);

    // Checks used by the service
    /// <summary>Returns the quoted Quantity of the item, or null if the QuotationItem does not exist.</summary>
    Task<int?> GetQuotedQuantityAsync(int quotationItemId);
    Task<bool> SizeExistsForQuotationItemAsync(int quotationItemId, string size, int? excludeSizeChartId = null);
    Task<int> GetTotalSizeQuantityAsync(int quotationItemId, int? excludeSizeChartId = null);
    Task<bool> IsQuotationItemLockedAsync(int quotationItemId);
}
