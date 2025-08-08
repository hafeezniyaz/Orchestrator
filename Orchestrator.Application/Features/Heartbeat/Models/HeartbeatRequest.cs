using System.ComponentModel.DataAnnotations;

namespace Orchestrator.Application.Features.Heartbeat.Models;

public class HeartbeatRequest
{
    [Required]
    public required string ClientId { get; set; }

    [MaxLength(200)]
    public string StatusMessage { get; set; } = "Active";
}