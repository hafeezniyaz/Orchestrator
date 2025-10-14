using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// represents a single log entry 
    /// </summary>
    [Table("TMUSLog")]
    public class Log : AuditableEntity
    {
        public DateTime TimeStamp { get; set; }
        public required string  Source { get; set; }
        public required string Level { get; set; }
        public required string Message { get; set; }
        public string? Hostname { get; set; }
        public string? UserId { get; set; }
    }
}
