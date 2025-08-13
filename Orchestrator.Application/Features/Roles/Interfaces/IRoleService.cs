using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.Roles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Roles.Interfaces
{
    public interface IRoleService
    {
        Task<PagedResult<RoleDto>> GetRolesAsync(string? name, int skip, int top);
    }
}
