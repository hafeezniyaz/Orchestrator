using Orchestrator.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Orchestrator.API.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                // Create a simple error response object
                var responseModel = new { message = error.Message };

                switch (error)
                {
                    case NotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case DuplicateResourceException:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        break;
                    default:
                        // Log the unhandled exception in detail
                        _logger.LogError(error, "An unhandled exception has occurred.");
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}