using INKED.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INKED.API.Controllers;

/// <summary>
/// Shared base class: turns service exceptions into consistent ProblemDetails responses
/// (404 not found, 400 invalid reference/rule, 409 conflict). Controllers never touch SQL.
/// </summary>
[ApiController]
public abstract class InkedControllerBase : ControllerBase
{
    protected async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (BadRequestException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Validation failed");
        }
        catch (ConflictException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status409Conflict, title: "Operation not allowed");
        }
        catch (DbUpdateException ex)
        {
            // Safety net: a foreign-key, unique or CHECK constraint was hit despite the service checks (e.g. a race).
            var logger = HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
            logger.LogError(ex, "Database constraint violation");

            return Problem(
                detail: "The change conflicts with a database constraint (foreign key, unique or check). Verify the values and try again.",
                statusCode: StatusCodes.Status409Conflict,
                title: "Database constraint violation");
        }
    }
}
