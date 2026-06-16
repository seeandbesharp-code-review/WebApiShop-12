using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PresidentsApp.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<ErrorHandlingMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception e)
            {
                logger.LogError($"Logged From My Middleware {e.Message}  {e.StackTrace}");
                httpContext.Response.StatusCode = 500;
                httpContext.Response.ContentType = "application/json";
                // Return RFC 7807 ProblemDetails-compatible JSON — do not leak stack trace
                await httpContext.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        type   = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        title  = "An unexpected error occurred.",
                        status = 500
                    }));
            }

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
