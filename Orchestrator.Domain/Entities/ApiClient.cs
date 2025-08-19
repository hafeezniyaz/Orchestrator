using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents a machine or service client that authenticates using the client_credentials flow.
    /// </summary>
    [Table("TMUSApiClient")]
    public class ApiClient : AuditableEntity
    {
        public required string ClientName { get; set; }
        public required string ClientId { get; set; }
        public required string HashedClientSecret { get; set; }

        /// <summary>
        /// A comma-separated list of roles assigned to this client (e.g., "Publisher,View").
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}