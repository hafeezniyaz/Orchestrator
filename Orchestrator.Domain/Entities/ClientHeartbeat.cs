using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// tracks the status and last seen time of the client machine
    /// </summary>
    [Table("TMUSClientHeartbeat")]    
    
    public class ClientHeartbeat: AuditableEntity
    {
        public required string ClientID  { get; set; }
        public DateTime LastHeartbeat { get; set; }
        public required string Status { get; set; }

    }
}
