using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents an immutable record of an action taken within the API for auditing purposes.
    /// </summary>
    public class AuditLog : BaseEntity
    {
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public required string Action { get; set; } // e.g., "POST /api/v1/apps"
        public string? RequestDetails { get; set; } // Can be a JSON representation of the request
        public required string IpAddress { get; set; }
        public int StatusCode { get; set; }
    } 
}
