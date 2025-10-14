using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.RoleMappings.Models
{
    public class SetADGroupRoleRequest
    {
        public required string RoleName { get; set; }
    }
}
