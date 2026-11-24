using INKED.Application.Interfaces;
using INKED.Application.Models.DTO.FeasibilityChecks;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers.FeasibilityChecks;

[ApiController]
[Route("api/feasibilitychecks")]
public class FeasibilityChecksController : InkedControllerBase
{
    private readonly IFeasibilityCheckService _service;

    public FeasibilityChecksController(IFeasibilityCheckService service) => _service = service;

    /// <summary>Gets all feasibility check records.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<FeasibilityCheckResponseDto>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetAll(CancellationToken ct)
        => ExecuteAsync(async () => Ok(await _service.GetAllAsync(ct)));

    /// <summary>Gets one feasibility check record by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FeasibilityCheckResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetById(int id, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null) return NotFound();
            return Ok(item);
        });

    /// <summary>Creates a new feasibility check record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FeasibilityCheckResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> Create(CreateFeasibilityCheckDto dto, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.FeasibilityId }, created);
        });

    /// <summary>Updates an existing feasibility check record.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Update(int id, UpdateFeasibilityCheckDto dto, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            if (!updated) return NotFound();
            return NoContent();
        });

    /// <summary>Deletes a feasibility check record (409 if other records depend on it).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Delete(int id, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var deleted = await _service.DeleteAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        });
}
