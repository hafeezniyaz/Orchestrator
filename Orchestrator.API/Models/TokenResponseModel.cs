using System.Text.Json.Serialization;
namespace Orchestrator.API.Models
{
    public class TokenResponseModel
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }
       
        [JsonPropertyName("expires_in")]
        public int ExperiseIn { get; set; }

        [JsonPropertyName("token_type")]
        public required string TokenType { get; set; }


    }
}
