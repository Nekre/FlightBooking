using System.Text.Json;
using FluentValidation;

namespace FlightBooking.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
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