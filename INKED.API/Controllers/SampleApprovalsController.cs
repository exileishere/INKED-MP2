using INKED.Application.DTOs;
using INKED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SampleApprovalsController : ControllerBase
{
    private readonly ISampleApprovalService _service;

    public SampleApprovalsController(ISampleApprovalService service) => _service = service;

    /// <summary>Get all SampleApprovals.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SampleApprovalResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SampleApprovalResponseDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Get one record by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SampleApprovalResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SampleApprovalResponseDto>> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    /// <summary>Create a new record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SampleApprovalResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SampleApprovalResponseDto>> Create([FromBody] CreateSampleApprovalDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.SampleApprovalID }, created);
    }

    /// <summary>Update an existing record.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SampleApprovalResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SampleApprovalResponseDto>> Update(int id, [FromBody] UpdateSampleApprovalDto dto)
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
