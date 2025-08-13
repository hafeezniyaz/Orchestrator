using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.ApiClients.Interfaces;
using Orchestrator.Application.Features.ApiClients.Models;
using Orchestrator.Application.Features.RoleMappings.Interfaces;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/clients")]
    [Authorize(Roles = "Administrator")] // IMPORTANT: This entire controller is locked to Administrators
    public class ApiClientsController : ControllerBase
    {
        private readonly IApiClientService _apiClientService;
        private readonly IRoleMappingService _roleMappingService;

        public ApiClientsController(
            IApiClientService apiClientService,
            IRoleMappingService roleMappingService)
        {
            _apiClientService = apiClientService;
            _roleMappingService = roleMappingService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApiClientRequest request)
        {
            // 2. Add the validation logic you designed
            var invalidRoles = await _roleMappingService.FindInvalidRolesAsync(request.RoleNames);
            if (invalidRoles.Any())
            {
                return BadRequest($"The following roles are invalid: {string.Join(", ", invalidRoles)}");
            }

            var response = await _apiClientService.CreateApiClientAsync(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }

        // 3. Add the new endpoint to update roles for an existing client
        [HttpPut("{apiClientId}/roles")]
        public async Task<IActionResult> UpdateApiClientRoles(Guid apiClientId, [FromBody] UpdateApiClientRolesRequest request)
        {
            var invalidRoles = await _roleMappingService.FindInvalidRolesAsync(request.RoleNames);
            if (invalidRoles.Any())
            {
                return BadRequest($"The following roles are invalid: {string.Join(", ", invalidRoles)}");
            }

            await _roleMappingService.UpdateApiClientRolesAsync(apiClientId, request.RoleNames);
            return NoContent(); // 204 No Content is a standard success response for a PUT
        }
    }
}