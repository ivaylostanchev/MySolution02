// Global middleware за хващане на всички неочаквани изключения.
// Връща стандартизиран JSON отговор вместо HTML/stack trace.

using System.Net;
using System.Text.Json;

namespace OperativeService.Middleware;

public class ItsExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ItsExceptionMiddleware> _logger;

    public ItsExceptionMiddleware(RequestDelegate next, ILogger<ItsExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Продължаваме към следващия middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Логваме грешката
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            // Връщаме стандартизиран JSON
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 500,
                message = "An unexpected error occurred.",
                detail = ex.Message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}