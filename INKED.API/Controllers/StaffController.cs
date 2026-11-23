using INKED.Application.DTOs;
using INKED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _service;

    public StaffController(IStaffService service) => _service = service;

    /// <summary>Get all Staff.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StaffResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StaffResponseDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Get one record by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StaffResponseDto>> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    /// <summary>Create a new record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StaffResponseDto>> Create([FromBody] CreateStaffDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.StaffID }, created);
    }

    /// <summary>Update an existing record.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StaffResponseDto>> Update(int id, [FromBody] UpdateStaffDto dto)
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
