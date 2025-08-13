using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.RoleMappings.Interfaces
{
    public interface IRoleMappingService
    {
        /// <summary>
        /// Checks a list of role names and returns any that are not found in the database.
        /// </summary>
        /// <param name="roleNames">The list of role names to validate.</param>
        /// <returns>A list of invalid role names. An empty list means all roles are valid.</returns>
        Task<IEnumerable<string>> FindInvalidRolesAsync(IEnumerable<string> roleNames);
        /// <summary>
        /// Deletes all existing role assignments for a given API client and
        /// replaces them with a new set of roles.
        /// </summary>
        /// <param name="apiClientId">The ID of the API client to update.</param>
        /// <param name="roleNames">An enumerable of role names to assign.</param>
        Task UpdateApiClientRolesAsync(Guid apiClientId, IEnumerable<string> roleNames);
    }
}
