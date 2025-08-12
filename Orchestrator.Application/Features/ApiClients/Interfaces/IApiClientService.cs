using Orchestrator.Application.Features.ApiClients.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.ApiClients.Interfaces
{
    public interface IApiClientService
    {
        Task<CreateApiClientResponse> CreateApiClientAsync(CreateApiClientRequest request);
    }
}
