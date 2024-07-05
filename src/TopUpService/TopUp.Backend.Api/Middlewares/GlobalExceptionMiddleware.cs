using Microsoft.AspNetCore.Mvc;

namespace Backend.TopUp.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, Serilog.ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext) 
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, httpContext);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails() { Title = "Some unknown error ocurred" });
            }
        }
    }

    public static class GlobalExceptionMiddlewareBuilder 
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
                => builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
