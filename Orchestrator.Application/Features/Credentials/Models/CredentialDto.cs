namespace Orchestrator.Application.Features.Credentials.Models
{
    /// <summary>
    /// A safe DTO for a credential that NEVER exposes the secret.
    /// Used for responses like 'Create'.
    /// </summary>
    public class CredentialDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Username { get; set; }
        public Guid AppId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}