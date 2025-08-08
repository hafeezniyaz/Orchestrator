using Orchestrator.Domain.Entities;

namespace Orchestrator.Domain.Entities
{
    public class ApiClient : BaseEntity
    {
        public required string ClientName { get; set; }
        public required string ClientId { get; set; }
        public required string HashedClientSecret { get; set; }
        // IMPROVEMENT: Changed to "Roles" to support multiple roles as a comma-separated string.
        public required string Roles { get; set; } // e.g., "Publisher,View"
        public bool IsActive { get; set; }
    }
}