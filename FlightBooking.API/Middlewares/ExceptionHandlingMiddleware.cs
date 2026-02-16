using System.Text.Json;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FlightBooking.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception. Method: {Method}, Path: {Path}, TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new { error = "Bir hata oluştu." };
        
        if (exception is ValidationException validationEx)
        {
            context.Response.StatusCode =StatusCodes.Status400BadRequest;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new 
            {
                Errors = validationEx.Errors.Select(e => e.ErrorMessage)
            }));
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsync(JsonSerializer.Serialize(new { Error = exception.Message }));
    }
}