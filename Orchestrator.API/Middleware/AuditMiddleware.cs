using System.Security.Claims;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Persistance;

namespace Orchestrator.API.Middleware
{

    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        
        public AuditMiddleware(RequestDelegate next)
        {
            
            _next = next;
        }

        //impliement InvokeAsync or Invoke method which will be called by the middleware with HttpContex as first argument
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering(); //to allow request stream to be read multiple times

            var auditLog = new AuditLog
            {
                Source = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                Action = $"{context.Request.Method} {context.Request.Path}",
                TimeStamp = DateTime.UtcNow,
                UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            
            //read request body
            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                auditLog.RequestDetails = await reader.ReadToEndAsync();

                context.Request.Body.Position = 0; //reset stream possition back to start for next middleware
            }

            await _next(context);  //call next middleware in pipeline

            //omce all middleware is called and response is generated 
            auditLog.StatusCode = context.Response.StatusCode;

            //save the auditlog to db 
            var dbContext = context.RequestServices.GetRequiredService<OrchestratorDbContext>(); // get the db context from servicess added to the app 
            await dbContext.AuditLogs.AddAsync(auditLog);
            await dbContext.SaveChangesAsync();
        }
    }
}
