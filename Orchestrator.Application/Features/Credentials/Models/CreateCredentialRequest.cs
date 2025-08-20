using System.ComponentModel.DataAnnotations;

namespace Orchestrator.Application.Features.Credentials.Models
{
    public class CreateCredentialRequest
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Username { get; set; }

        [Required]
        public required string Secret { get; set; }
    }
}