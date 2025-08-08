using System.ComponentModel.DataAnnotations;

namespace Orchestrator.Application.Features.Apps.Models;

public class UpdateAppRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}