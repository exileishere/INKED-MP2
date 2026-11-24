using System.ComponentModel.DataAnnotations;

namespace INKED.Application.SizeCharts;

/// <summary>
/// Data the client may change on a size row. QuotationItemId is the parent link and is
/// protected: to move a size to another item, delete the row and create a new one.
/// </summary>
public class UpdateSizeChartDto
{
    /// <summary>VARCHAR(10), not blank.</summary>
    [Required, StringLength(10, MinimumLength = 1)]
    public string Size { get; set; } = string.Empty;

    /// <summary>Must be greater than 0.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
}
