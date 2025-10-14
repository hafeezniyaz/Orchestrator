using Orchestrator.Application.Features.RoleMappings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.RoleMappings.Interfaces
{
    public interface IRoleMappingSevice
    {
        /// <summary>
        /// Update roles for API clients
        /// </summary>
        /// <param name="apiClient"></param>
        /// <param name="roleNames"></param>
        /// <returns></returns>
        Task UpdateApiClientRolesAsync(Guid apiClient, IEnumerable<string> roleNames);

        /// <summary>
        /// Returns emty list if all requested roles are valid, if not returns list of invalid roles
        /// </summary>
        /// <param name="roleNames"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> FindInvalidRolesAsync(IEnumerable<string> roleNames);

        /// <summary>
        /// Get All mappings for AD Group
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ADRoleMappingDto>> GetADRoleMappingsAsync();

        /// <summary>
        /// Set role for an AD Group
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task SetADGroupRoleAsync(string GroupName, string roleName);

        /// <summary>
        /// Remove add group from Role mapping
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        Task RemoveADGroupRoleAsync(string GroupName);
    }
}
