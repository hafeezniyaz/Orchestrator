using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents a sensitive credential (e.g., password, API key) associated with an App.
    /// The secret is always stored in an encrypted format.
    /// </summary>
    [Table("TMUSCredential")]
    public class Credential : AuditableEntity
    {
        /// <summary>
        /// The unique name of the credential for a given app (e.g., "DatabasePassword").
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// An optional description of the credential's purpose.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The username or identifier associated with the secret.
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// The encrypted secret, stored as a Base64 string in the format {IV}:{CipherText}:{AuthTag}.
        /// This should never contain plain text.
        /// </summary>
        public required string EncryptedSecret { get; set; }

        /// <summary>
        /// A foreign key referencing the App this credential belongs to.
        /// </summary>
        public Guid AppId { get; set; }
    }
}