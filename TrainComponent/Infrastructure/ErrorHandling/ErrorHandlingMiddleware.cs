using System.Net;
using System.Text.Json;

namespace TrainComponent.Infrastructure.ErrorHandling;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error = new ErrorResponse
            {
                Status = response.StatusCode,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            };

            var json = JsonSerializer.Serialize(error);
            await response.WriteAsync(json);
        }
    }
}
