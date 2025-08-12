using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Configs.Interfaces;
using System.Text.Json;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    // The route now includes the appId, making it clear configs are a sub-resource of an app.
    [Route("api/v1/apps/{appId}/configs")]
    [Authorize]
    public class ConfigsController : ControllerBase
    {
        private readonly IConfigService _configService;

        public ConfigsController(IConfigService configService)
        {
            _configService = configService;
        }

        // POST /api/v1/apps/{appId}/configs/{configName}
        [HttpPost("{configName}")]
        public async Task<IActionResult> CreateConfigContainer([FromRoute] Guid appId, string configName)
        {
            await _configService.CreateConfigContainerAsync(configName, appId);
            return CreatedAtAction(nameof(GetConfig), new { appId, configName }, null);
        }

        // GET /api/v1/apps/{appId}/configs/{configName}
        [HttpGet("{configName}")]
        public async Task<IActionResult> GetConfig([FromRoute] Guid appId, string configName)
        {
            var config = await _configService.GetConfigAsync(configName, appId);
            return Ok(config);
        }

        // GET /api/v1/apps/{appId}/configs/{configName}/assets/{key}
        [HttpGet("{configName}/assets/{key}")]
        public async Task<IActionResult> GetAsset([FromRoute] Guid appId, string configName, string key)
        {
            var asset = await _configService.GetAssetAsync(configName, key, appId);
            return asset.HasValue ? Ok(asset.Value) : NotFound();
        }

        // PUT /api/v1/apps/{appId}/configs/{configName}/assets/{key}
        [HttpPut("{configName}/assets/{key}")]
        public async Task<IActionResult> SetAsset([FromRoute] Guid appId, string configName, string key, [FromBody] JsonElement value)
        {
            await _configService.SetAssetAsync(configName, key, value, appId);
            return NoContent();
        }

        // DELETE /api/v1/apps/{appId}/configs/{configName}
        [HttpDelete("{configName}")]
        public async Task<IActionResult> DeleteConfig([FromRoute] Guid appId, string configName)
        {
            await _configService.DeleteConfigContainerAsync(configName, appId);
            return NoContent();
        }

        // DELETE /api/v1/apps/{appId}/configs/{configName}/assets/{key}
        [HttpDelete("{configName}/assets/{key}")]
        public async Task<IActionResult> DeleteAsset([FromRoute] Guid appId, string configName, string key)
        {
            await _configService.DeleteAssetAsync(configName, key, appId);
            return NoContent();
        }
    }
}