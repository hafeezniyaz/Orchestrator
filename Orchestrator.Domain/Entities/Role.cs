using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    [Table("TMUSRole")]
    public class Role : AuditableEntity
    {
        public required string Name { get; set; }
    }
}
