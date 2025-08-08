using System.Text.Json;

namespace Orchestrator.Application.Features.Configs.Models
{
    public class ConfigResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? AppId { get; set; }
        public Dictionary<string, JsonElement> Assets { get; set; } = new();
    }

    public class AssetResponse
    {
        public Guid Id { get; set; }
        public Guid ConfigId { get; set; }
        public string Key { get; set; } = string.Empty;
        public JsonElement Value { get; set; }
    }
}