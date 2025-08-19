using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents a managed application, which acts as a container for packages.
    /// </summary>
    /// 

    [Table("TMUSApp")]
    public class App : AuditableEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
 