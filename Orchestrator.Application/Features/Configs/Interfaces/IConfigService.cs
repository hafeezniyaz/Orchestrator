using System.Text.Json;
using Orchestrator.Application.Features.Configs.Models;

namespace Orchestrator.Application.Features.Configs.Interfaces
{
    public interface IConfigService
    {
        Task<ConfigResponse?> GetConfigAsync(string configName, Guid? appId);
        Task<AssetResponse> SetAssetAsync(string configName, string key, JsonElement value, Guid? appId);
        Task<bool> DeleteAssetAsync(string configName, string key, Guid? appId);
        Task<bool> DeleteConfigAsync(string configName, Guid? appId);
    }
}