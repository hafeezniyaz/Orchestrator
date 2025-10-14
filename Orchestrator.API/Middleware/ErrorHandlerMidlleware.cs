using Orchestrator.API.Models;
using Orchestrator.Application.Common.Exceptions;
using System.Data;
using System.Net;
using System.Text.Json;

namespace Orchestrator.API.Middleware
{
    public class ErrorHandlerMidlleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMidlleware> _logger;

        public ErrorHandlerMidlleware( RequestDelegate next, ILogger<ErrorHandlerMidlleware> logger )
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
            catch(Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                ErrorResponse errorResponse;

                switch(ex)
                {
                    case NotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        errorResponse = new ErrorResponse { Error = "not_found", ErrorDesciption = ex.Message };
                        break;

                    case DuplicateResourceException:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        errorResponse = new ErrorResponse { Error = "duplicate_item", ErrorDesciption = ex.Message };
                        break;

                    default:
                        _logger.LogError(ex, "An unhandled exception has occured");
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorResponse = new ErrorResponse { Error = "internal_error", ErrorDesciption = ex.Message };
                        break;
                }

                var result = JsonSerializer.Serialize(errorResponse);
                await response.WriteAsync(result);
            }
        }
    }
}
