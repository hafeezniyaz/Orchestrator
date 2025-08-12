using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.ApiClients.Interfaces;
using Orchestrator.Application.Features.ApiClients.Models;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/clients")]
    [Authorize(Roles = "Administrator")] // IMPORTANT: This entire controller is locked to Administrators
    public class ApiClientsController : ControllerBase
    {
        private readonly IApiClientService _apiClientService;

        public ApiClientsController(IApiClientService apiClientService)
        {
            _apiClientService = apiClientService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApiClientRequest request)
        {
            var response = await _apiClientService.CreateApiClientAsync(request);

            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }
    }
}