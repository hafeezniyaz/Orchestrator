using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.ApiClients.Models
{
    public  class UpdateApiClientRolesRequest
    {
        public required List<string> RoleNames { get; set; }
    }
}
