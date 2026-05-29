namespace IndieVault.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex) // Catch any unhandled exceptions
            {
                // Log the exception details using the logger
                _logger.LogCritical(ex, "Error");

                // Log the exception details to a file named "errors.log" in the content root directory
                var logPath = Path.Combine(_env.ContentRootPath, "errors.log");

                var logMessage = $"[{DateTime.UtcNow}] {ex.Message}, Action: {context.Request.Path}, Location: {ex.StackTrace}, Method: {context.Request.Method}{Environment.NewLine}";

                // Append the log message to the file asynchronously
                await File.AppendAllTextAsync(logPath, logMessage);

                if (context.Response.HasStarted) // Check if the response has already started, if so, we can't modify it
                {
                    return;
                }

                // Set the response status code to 500 (Internal Server Error) and redirect to a custom error page  
                context.Response.StatusCode = 500; 
                context.Response.Redirect("/Error");
            }
        }
    }
}
