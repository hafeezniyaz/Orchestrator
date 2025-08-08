using System.Security.Claims;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Persistence;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // the middleware looks for the method called InvokeAsync or Invoke when a miiddleware is registered 
    public async Task InvokeAsync(HttpContext context) 
    {
        // Allow the request stream to be read multiple times
        context.Request.EnableBuffering();

        var auditLog = new AuditLog
        {
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            Action = $"{context.Request.Method} {context.Request.Path}",
            Timestamp = DateTime.UtcNow,
            // Extract user ID from the token's claims if the user is authenticated
            UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        // Read request body
        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            auditLog.RequestDetails = await reader.ReadToEndAsync();
            // IMPORTANT: Reset stream position for the next middleware
            context.Request.Body.Position = 0;
        }

        await _next(context); // Call the next middleware in the pipeline

        // After the request is handled, capture the response status code
        auditLog.StatusCode = context.Response.StatusCode;

        // Save the audit log to the database
        var dbContext = context.RequestServices.GetRequiredService<OrchestratorDbContext>();
        await dbContext.AuditLogs.AddAsync(auditLog);
        await dbContext.SaveChangesAsync();
    }
}