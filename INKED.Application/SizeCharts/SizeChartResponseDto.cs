namespace INKED.Application.SizeCharts;

/// <summary>Data returned by the API for a size chart row.</summary>
public class SizeChartResponseDto
{
    public int SizeChartId { get; set; }
    public int QuotationItemId { get; set; }
    public string Size { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
