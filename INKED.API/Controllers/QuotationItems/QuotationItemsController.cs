using INKED.Application.Interfaces;
using INKED.Application.Models.DTO.QuotationItems;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers.QuotationItems;

[ApiController]
[Route("api/quotationitems")]
public class QuotationItemsController : InkedControllerBase
{
    private readonly IQuotationItemService _service;

    public QuotationItemsController(IQuotationItemService service) => _service = service;

    /// <summary>Gets all quotation item records.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<QuotationItemResponseDto>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetAll(CancellationToken ct)
        => ExecuteAsync(async () => Ok(await _service.GetAllAsync(ct)));

    /// <summary>Gets one quotation item record by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(QuotationItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetById(int id, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null) return NotFound();
            return Ok(item);
        });

    /// <summary>Creates a new quotation item record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(QuotationItemResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> Create(CreateQuotationItemDto dto, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.QuotationItemId }, created);
        });

    /// <summary>Updates an existing quotation item record.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Update(int id, UpdateQuotationItemDto dto, CancellationToken ct)
        => ExecuteAsync(async () =>
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            if (!updated) return NotFound();
            return NoContent();
        });

    /// <summary>Deletes a quotation item record (409 if other records depend on it).</summary>
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
