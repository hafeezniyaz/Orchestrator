using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents a configuration container, which can be global or app-specific.
    /// </summary>
    [Table("TMUSConfig")]
    public class Config: AuditableEntity
    {
        public required string Name { get; set; }

        // If AppId is null, it's a global configuration.
        public required Guid AppId { get; set; }
    }
}
