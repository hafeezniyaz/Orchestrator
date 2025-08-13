using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    public class Role : AuditableEntity
    {
        public required string Name { get; set; }
    }
}
