// INKED MP2 | Table: Invoices | Layer: API / Controller
// Owner: Rashel Maxine Baquirin (Billing module)
using INKED.Application.Billing;
using INKED.Application.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers;

[ApiController]
[Route("api/invoices")]
[Produces("application/json")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoicesController(IInvoiceService service)
    {
        _service = service;
    }

    /// <summary>Retrieve all invoices.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<InvoiceResponseDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Retrieve one invoice by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound(NotFoundProblem(id)) : Ok(item);
    }

    /// <summary>Create a new invoice.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Create(CreateInvoiceDto dto) => Handle(async () =>
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.InvoiceId }, created);
    });

    /// <summary>Update an existing invoice.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Update(int id, UpdateInvoiceDto dto) => Handle(async () =>
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound(NotFoundProblem(id));
    });

    /// <summary>Delete a invoice.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Delete(int id) => Handle(async () =>
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound(NotFoundProblem(id));
    });

    // Turns business-rule errors into 400 (validation) or 409 (conflict) responses.
    private async Task<IActionResult> Handle(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ValidationFailedException ex)
        {
            return ValidationProblem(new ValidationProblemDetails(ex.Errors)
            {
                Title = "One or more validation errors occurred.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (ConflictException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Conflict",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    private ProblemDetails NotFoundProblem(int id) => new()
    {
        Title = "Record not found",
        Status = StatusCodes.Status404NotFound,
        Detail = $"Invoice with ID {id} was not found."
    };
}
