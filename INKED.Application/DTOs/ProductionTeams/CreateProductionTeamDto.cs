using System.ComponentModel.DataAnnotations;

namespace INKED.Application.DTOs;

public class CreateProductionTeamDto
{
    [Required, StringLength(40, MinimumLength = 1)]
    public string TeamName { get; set; } = string.Empty;
}
