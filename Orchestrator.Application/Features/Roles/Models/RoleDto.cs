using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Roles.Models
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
