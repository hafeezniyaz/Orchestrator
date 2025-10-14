using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    [Table("TMUSAuditLog")]
    public class AuditLog : BaseEntity
    {
        public DateTime TimeStamp { get; set; }
        public string? UserId { get; set; }

        public required string Action { get; set; }

        public string? RequestDetails { get; set; }

        public required string Source { get; set; }

        public int StatusCode { get; set; }
    }
}
