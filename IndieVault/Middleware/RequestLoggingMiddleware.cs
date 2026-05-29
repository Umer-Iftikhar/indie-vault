using System.Security.Claims;

namespace IndieVault.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User; // Access the ClaimsPrincipal representing the current user
            var userName = "Anonymous"; 

            if (user.Identity?.IsAuthenticated == true) // Check if the user is authenticated
            {
                userName = user.Identity.Name ?? "Anonymous"; // Get the username from the identity, or fallback to "Anonymous" if it's null
            }

            string requestPath = context.Request.Path; // Get the request path
            if (requestPath.Contains("."))
            {
                await _next(context); // Skip logging for static file requests
            }
            else
            {
                // Log the incoming request with method, path, and user information
                _logger.LogInformation($"Incoming Request: " +
                $"{context.Request.Method} {requestPath}" +
                $" User {userName}");

                await _next(context); // Call the next middleware in the pipeline

                // Log the outgoing response with method, path, and status code
                _logger.LogInformation($"Outgoing Response: " +
                    $"{context.Request.Method} {requestPath}" +
                    $" StatusCode: {context.Response.StatusCode}");
            }

            
        }
    }
}
