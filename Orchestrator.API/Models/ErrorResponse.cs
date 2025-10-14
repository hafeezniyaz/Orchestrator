using System.Text.Json.Serialization;

namespace Orchestrator.API.Models
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public required string Error { get; set; }

        [JsonPropertyName("error_description")]
        public required string ErrorDesciption { get; set; }
    }
}
