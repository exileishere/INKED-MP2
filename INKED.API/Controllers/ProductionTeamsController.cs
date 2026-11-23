using INKED.Application.DTOs;
using INKED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductionTeamsController : ControllerBase
{
    private readonly IProductionTeamService _service;

    public ProductionTeamsController(IProductionTeamService service) => _service = service;

    /// <summary>Get all ProductionTeams.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductionTeamResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductionTeamResponseDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Get one record by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductionTeamResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductionTeamResponseDto>> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    /// <summary>Create a new record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductionTeamResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductionTeamResponseDto>> Create([FromBody] CreateProductionTeamDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.TeamID }, created);
    }

    /// <summary>Update an existing record.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductionTeamResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductionTeamResponseDto>> Update(int id, [FromBody] UpdateProductionTeamDto dto)
        => Ok(await _service.UpdateAsync(id, dto));

    /// <summary>Delete a record.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
