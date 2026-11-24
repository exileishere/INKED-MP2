using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Models.DTO.Inquiries;

/// <summary>CustomerId and DateReceived are fixed once an inquiry is created.</summary>
public class UpdateInquiryDto
{
    [Range(1, int.MaxValue)]
    public int? HandledByUserId { get; set; }

    [Required, StringLength(30)]
    public string InquiryChannel { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? InquiryDetails { get; set; }

    public bool NeedsDesignHelp { get; set; }

    public DateTime? FirstResponseAt { get; set; }

    [Required, StringLength(20)]
    public string Outcome { get; set; } = string.Empty;
}
