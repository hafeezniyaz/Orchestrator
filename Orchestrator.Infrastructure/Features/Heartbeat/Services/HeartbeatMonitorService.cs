using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrator.Infrastructure.Persistence;

namespace Orchestrator.Infrastructure.Features.Heartbeat.Services;

/// <summary>
/// A background service that periodically checks the status of client heartbeats.
/// It inherits from BackgroundService, which is the standard base class for IHostedService.
/// </summary>
public class HeartbeatMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// We inject IServiceProvider to be able to create our own DI scopes,
    /// which is necessary because this service is a singleton while DbContext is scoped.
    /// </summary>
    public HeartbeatMonitorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// This method is the entry point for the background service.
    /// It will run for the entire lifetime of the application.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // A modern, async-friendly timer that runs every 1 minute.
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        // The loop will continue until the application is requested to stop.
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CheckHeartbeatsAsync();
        }
    }

    private async Task CheckHeartbeatsAsync()
    {
        // Here we create a new DI scope to safely resolve our scoped DbContext.
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrchestratorDbContext>();

        var now = DateTime.UtcNow;
        var twoMinutesAgo = now.AddMinutes(-2);
        var fiveMinutesAgo = now.AddMinutes(-5);

        // Find all heartbeats that haven't checked in for over 2 minutes.
        var heartbeatsToUpdate = await dbContext.ClientHeartbeats
            .Where(h => h.Status == "Active" && h.LastHeartbeat < twoMinutesAgo)
            .ToListAsync();

        if (!heartbeatsToUpdate.Any())
        {
            return; // Nothing to do, exit early.
        }

        // Update the status based on the PRD's business rules.
        foreach (var heartbeat in heartbeatsToUpdate)
        {
            if (heartbeat.LastHeartbeat < fiveMinutesAgo)
            {
                heartbeat.Status = "Disconnected";
            }
            else // It must be between 2 and 5 minutes ago.
            {
                heartbeat.Status = "Not Active";
            }
        }

        await dbContext.SaveChangesAsync();
    }
}