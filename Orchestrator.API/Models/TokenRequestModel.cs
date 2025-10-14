using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Orchestrator.API.Models
{
    public class TokenRequestModel
    {
        [FromForm(Name ="grant_type")]
        public required string GrantType { get; set; }

        [FromForm(Name = "username")]
        public string? Username { get; set; }

        [FromForm(Name = "password")]
        public string? Password { get; set; }

        [FromForm(Name = "client_id")]
        public Guid? ClientId { get; set; }

        [FromForm(Name = "client_secret")]
        public string? ClientSecret { get; set; }

        [FromForm(Name = "scope")]
        public string? Scope { get; set; }


    }
}
