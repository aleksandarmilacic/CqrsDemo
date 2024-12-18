namespace CqrsDemo.Api.Helpers
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public DateTime Timestamp { get; set; }
    }



    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorResponse = new ErrorResponse
            {
                Timestamp = DateTime.UtcNow,
                Message = exception.Message,
                Detail = context.Request.Host.Value 
            };

            context.Response.ContentType = "application/json";

            switch (exception)
            {
                case ArgumentException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    errorResponse.ErrorCode = "BAD_REQUEST";
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    errorResponse.ErrorCode = "UNAUTHORIZED";
                    break;
                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    errorResponse.ErrorCode = "NOT_FOUND";
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    errorResponse.ErrorCode = "SERVER_ERROR";
                    break;
            }

            var result = System.Text.Json.JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(result);
        }
    }


}
