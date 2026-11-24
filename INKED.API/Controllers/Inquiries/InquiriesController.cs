using INKED.Application.Interfaces;
using INKED.Application.Models.DTO.Inquiries;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers.Inquiries;

[ApiController]
[Route("api/inquiries")]
public class InquiriesController : InkedControllerBase
{
    private readonly IInquiryService _service;

    public InquiriesController(IInquiryService service) => _service = service;

    /// <summary>Gets all inquiry records.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<InquiryResponseDto>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetAll(CancellationToken ct)
        => ExecuteAsync(async () => Ok(await _service.GetAllAsync(ct)));

    /// <summary>Gets one inquiry record by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InquiryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetById(int id, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null) return NotFound();
            return Ok(item);
        });

    /// <summary>Creates a new inquiry record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(InquiryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> Create(CreateInquiryDto dto, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.InquiryId }, created);
        });

    /// <summary>Updates an existing inquiry record.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Update(int id, UpdateInquiryDto dto, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            if (!updated) return NotFound();
            return NoContent();
        });

    /// <summary>Deletes a inquiry record (409 if other records depend on it).</summary>
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
