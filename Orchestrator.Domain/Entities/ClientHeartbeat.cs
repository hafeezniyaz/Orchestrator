using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Tracks the status and last seen time of a client machine.
    /// </summary>
    public class ClientHeartbeat : BaseEntity
    {
        public required string ClientId { get; set; } // A unique identifier for the machine/client
        public DateTime LastHeartbeat { get; set; }
        public required string Status { get; set; }
    }
}
