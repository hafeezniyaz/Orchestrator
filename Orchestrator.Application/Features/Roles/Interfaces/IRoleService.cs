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
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name="name"></param>
        /// <param name="skip"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        Task<PagedResult<RoleDto>> GetRolesAsync(string? name, int skip = 0, int top = 50);
    }
}
