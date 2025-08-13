using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    public class RoleMapping : AuditableEntity
    {
        [Required]
        public Guid RoleId { get; set; }

        // A mapping can be for an ApiClient OR an ADGroup, but not both.
        public Guid? ApiClientId { get; set; }
        public string? ADGroup { get; set; }
    }
}
