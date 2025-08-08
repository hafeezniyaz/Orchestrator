using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents a single key-value pair within a Config container.
    /// The value is stored as a JSON string to allow for complex objects.
    /// </summary>
    public class ConfigAsset: BaseEntity
    {
        public Guid ConfigId { get; set; }
        public required string Key { get; set; }
        public required string ValueJson { get; set; }
    }
}
