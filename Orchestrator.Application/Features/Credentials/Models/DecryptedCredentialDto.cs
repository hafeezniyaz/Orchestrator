namespace Orchestrator.Application.Features.Credentials.Models
{
    /// <summary>
    /// A DTO that includes the decrypted, plain-text secret.
    /// This should only be used for responses where the user explicitly requests the secret.
    /// </summary>
    public class DecryptedCredentialDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Username { get; set; }
        public Guid AppId { get; set; }

        /// <summary>
        /// The plain-text, decrypted secret.
        /// </summary>
        public required string Secret { get; set; }
    }
}