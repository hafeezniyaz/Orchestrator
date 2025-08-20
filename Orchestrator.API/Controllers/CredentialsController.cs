using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Credentials.Interfaces;
using Orchestrator.Application.Features.Credentials.Models;
using System;
using System.Threading.Tasks;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/apps/{appId}/credentials")]
    [Authorize(Roles = "Administrator")] // 1. Secure all endpoints in this controller
    public class CredentialsController : ControllerBase
    {
        private readonly ICredentialService _credentialService;

        public CredentialsController(ICredentialService credentialService)
        {
            _credentialService = credentialService;
        }

        /// <summary>
        /// Creates a new credential for a specific application.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromRoute] Guid appId,
            [FromBody] CreateCredentialRequest request)
        {
            var newCredentialDto = await _credentialService.CreateCredentialAsync(appId, request);

            // 2. Return a 201 Created response, a standard RESTful practice.
            return CreatedAtAction(
                nameof(GetByName),
                new { appId = newCredentialDto.AppId, credentialName = newCredentialDto.Name },
                newCredentialDto);
        }

        /// <summary>
        /// Retrieves a single credential, including its decrypted secret.
        /// </summary>
        [HttpGet("{credentialName}")]
        public async Task<IActionResult> GetByName(
            [FromRoute] Guid appId,
            [FromRoute] string credentialName)
        {
            // 3. Notice the clean code: no try/catch or if/null checks needed here!
            var credential = await _credentialService.GetCredentialAsync(appId, credentialName);
            return Ok(credential);
        }

        /// <summary>
        /// Updates an existing credential's details and secret.
        /// </summary>
        [HttpPut("{credentialName}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid appId,
            [FromRoute] string credentialName,
            [FromBody] UpdateCredentialRequest request)
        {
            await _credentialService.UpdateCredentialAsync(appId, credentialName, request);

            // 4. Return a 204 No Content response, standard for a successful update.
            return NoContent();
        }

        /// <summary>
        /// Deletes a credential permanently.
        /// </summary>
        [HttpDelete("{credentialName}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid appId,
            [FromRoute] string credentialName)
        {
            await _credentialService.DeleteCredentialAsync(appId, credentialName);
            return NoContent();
        }
    }
}