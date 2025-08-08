using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Configs.Interfaces;
using System.Text.Json;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/configs")]
    [Authorize]
    public class ConfigsController : ControllerBase
    {
        private readonly IConfigService _configService;

        public ConfigsController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet("{configName}")]
        [Authorize(Roles = "User,Administrator,View,Publisher")]
        public async Task<IActionResult> GetConfig(string configName, [FromQuery] Guid? appId)
        {
            var config = await _configService.GetConfigAsync(configName, appId);
            
            if (config == null)
            {
                return NotFound($"Config '{configName}' not found.");
            }

            return Ok(config);
        }

        [HttpPut("{configName}/assets/{key}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SetAsset(string configName, string key, [FromBody] JsonElement value, [FromQuery] Guid? appId)
        {
            var asset = await _configService.SetAssetAsync(configName, key, value, appId);
            return Ok(asset);
        }

        [HttpDelete("{configName}/assets/{key}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAsset(string configName, string key, [FromQuery] Guid? appId)
        {
            var success = await _configService.DeleteAssetAsync(configName, key, appId);
            
            if (!success)
            {
                return NotFound($"Asset '{key}' not found in config '{configName}'.");
            }

            return NoContent();
        }

        [HttpDelete("{configName}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfig(string configName, [FromQuery] Guid? appId)
        {
            var success = await _configService.DeleteConfigAsync(configName, appId);
            
            if (!success)
            {
                return NotFound($"Config '{configName}' not found.");
            }

            return NoContent();
        }
    }
}