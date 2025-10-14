using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.ApiClient.Interfaces
{
    public interface IApiClientService
    {
        /// <summary>
        /// create client api
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CreateClientApiResponse> CreateApiClient(CreateApiClientRequest request);

        /// <summary>
        /// List all api clients
        /// </summary>
        /// <param name="name"></param>
        /// <param name="skip"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        Task<PagedResult<ApiClientDto>> ListAllApiClients(string? name, int skip = 0, int top = 50);

        /// <summary>
        /// Delete a client based on client id
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        Task DeleteClient(Guid ClientId);
    }
}
