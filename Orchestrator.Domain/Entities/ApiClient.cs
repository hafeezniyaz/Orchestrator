using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    [Table("TMUSApiClient")]
    public class ApiClient : AuditableEntity
    {
        public required  string  ClientName{ get; set; }
        public required Guid ClientId { get; set; }
        public required string HashedClientSecret { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
