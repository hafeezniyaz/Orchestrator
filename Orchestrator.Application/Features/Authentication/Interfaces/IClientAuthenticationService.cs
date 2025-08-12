using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Authentication.Interfaces
{
    /// <summary>
    /// Defines a service for handling the business logic of client authentication.
    /// </summary>
    public interface IClientAuthenticationService
    {
        /// <summary>
        /// Authenticates a client using the client_credentials grant type.
        /// </summary>
        /// <param name="clientId">The client's public ID.</param>
        /// <param name="clientSecret">The client's plain-text secret.</param>
        /// <returns>A valid JWT token if authentication is successful; otherwise, null.</returns>
        Task<string?> AuthenticateClientCredentialsAsync(string? clientId, string? clientSecret);
    }
}
