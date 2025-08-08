using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Features.Heartbeat.Interfaces;
using Orchestrator.Application.Features.Heartbeat.Models;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Persistence;

namespace Orchestrator.Infrastructure.Features.Heartbeat.Services;

public class HeartbeatService : IHeartbeatService
{
    private readonly OrchestratorDbContext _context;

    public HeartbeatService(OrchestratorDbContext context)
    {
        _context = context;
    }

    public async Task RecordHeartbeatAsync(HeartbeatRequest request)
    {
        var existingHeartbeat = await _context.ClientHeartbeats
            .FirstOrDefaultAsync(h => h.ClientId == request.ClientId);

        if (existingHeartbeat != null)
        {
            // UPDATE existing record
            existingHeartbeat.LastHeartbeat = DateTime.UtcNow;
            existingHeartbeat.Status = request.StatusMessage;
        }
        else
        {
            // INSERT new record
            var newHeartbeat = new ClientHeartbeat
            {
                Id = Guid.NewGuid(),
                ClientId = request.ClientId,
                LastHeartbeat = DateTime.UtcNow,
                Status = request.StatusMessage
            };
            _context.ClientHeartbeats.Add(newHeartbeat);
        }

        await _context.SaveChangesAsync();
    }
}