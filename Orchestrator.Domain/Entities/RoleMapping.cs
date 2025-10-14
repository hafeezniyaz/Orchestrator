using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    [Table("TMUSRoleMapping")]
    public class RoleMapping : AuditableEntity
    {
        public Guid RoleId { get; set; }
        public Guid? ApiClientId { get; set; }
        public string? AdGroup { get; set; }
    }
}
