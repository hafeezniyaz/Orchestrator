using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that generates JSON Web Tokens (JWT).
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a JWT for a given user with their associated roles.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="roles">A collection of roles assigned to the user.</param>
        /// <returns>A signed JWT string.</returns>
        string GenerateToken(Guid userId, string username, IEnumerable<string> roles);
    }
}
