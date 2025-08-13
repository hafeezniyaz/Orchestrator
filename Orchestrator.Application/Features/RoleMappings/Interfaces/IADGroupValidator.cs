using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.RoleMappings.Interfaces
{
    /// <summary>
    /// Defines a service for validating Active Directory group names.
    /// </summary>
    public interface IADGroupValidator
    {
        /// <summary>
        /// Checks if a given group name exists in the current Active Directory domain.
        /// </summary>
        /// <param name="groupName">The name of the group to validate (e.g., "YOUR_DOMAIN\AppPublishers").</param>
        /// <returns>True if the group exists, otherwise false.</returns>
        bool DoesGroupExist(string groupName);
    }
}
