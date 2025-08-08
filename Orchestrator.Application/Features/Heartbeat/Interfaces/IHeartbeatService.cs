using Orchestrator.Application.Features.Heartbeat.Models;

namespace Orchestrator.Application.Features.Heartbeat.Interfaces;

public interface IHeartbeatService
{
    Task RecordHeartbeatAsync(HeartbeatRequest request);
}