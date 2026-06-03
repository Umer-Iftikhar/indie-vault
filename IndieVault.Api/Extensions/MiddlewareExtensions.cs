using IndieVault.Api.Middleware;

namespace IndieVault.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
