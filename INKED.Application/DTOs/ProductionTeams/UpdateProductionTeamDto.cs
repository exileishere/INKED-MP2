using System.ComponentModel.DataAnnotations;

namespace INKED.Application.DTOs;

public class UpdateProductionTeamDto
{
    [Required, StringLength(40, MinimumLength = 1)]
    public string TeamName { get; set; } = string.Empty;
}
