using INKED.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INKED.API.Middleware;

/// <summary>
/// Turns service-layer exceptions into clean HTTP responses (404 / 400 / 409)
/// so controllers stay thin and never leak stack traces.
/// SHARED FILE: add it once to INKED.API (coordinate with Ileana).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)   { await Write(context, StatusCodes.Status404NotFound, "Not Found", ex.Message); }
        catch (BadRequestException ex) { await Write(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message); }
        catch (ConflictException ex)   { await Write(context, StatusCodes.Status409Conflict, "Conflict", ex.Message); }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Database constraint violation.");
            await Write(context, StatusCodes.Status409Conflict, "Conflict",
                "The operation violates a database constraint (foreign key, unique, or check).");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            await Write(context, StatusCodes.Status500InternalServerError, "Server Error",
                "An unexpected error occurred.");
        }
    }

    private static Task Write(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        });
    }
}
