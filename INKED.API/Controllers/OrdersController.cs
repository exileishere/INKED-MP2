// INKED MP2 | Table: Orders | Layer: API / Controller
// Owner: Rashel Maxine Baquirin (Billing module)
using INKED.Application.Billing;
using INKED.Application.Orders;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers;

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    /// <summary>Retrieve all orders.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Retrieve one order by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound(NotFoundProblem(id)) : Ok(item);
    }

    /// <summary>Create a new order.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Create(CreateOrderDto dto) => Handle(async () =>
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
    });

    /// <summary>Update an existing order.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Update(int id, UpdateOrderDto dto) => Handle(async () =>
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound(NotFoundProblem(id));
    });

    /// <summary>Delete a order.</summary>
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
        Detail = $"Order with ID {id} was not found."
    };
}
