using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents a single log entry, typically submitted by a client application.
    /// </summary>
    public class Log : BaseEntity
    {
        public DateTime Timestamp { get; set; }
        public required string Source { get; set; }
        public required string Level { get; set; }
        public required string Message { get; set; }
        public string? Hostname { get; set; }
        public string? UserId { get; set; }
    }
}
