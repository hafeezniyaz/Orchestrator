using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// A base class for all entities to provide a common primary key.
    /// Using Guid ensures that IDs are unique across the entire system,
    /// which is better than auto-incrementing integers in a distributed environment.
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
    }
}
