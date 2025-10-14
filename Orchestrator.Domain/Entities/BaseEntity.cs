using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// A base class for all entities to provide a common primary key 
    /// </summary>

    public abstract class  BaseEntity
    {
        public Guid Id { get; set; }
    }
}
