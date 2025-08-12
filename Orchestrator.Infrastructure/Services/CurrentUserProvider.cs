using Microsoft.AspNetCore.Http;
using Orchestrator.Application.Common.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Orchestrator.Infrastructure.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.Claims
                                   .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public string? UserName => _httpContextAccessor.HttpContext?.User?.Claims
                                   .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    }
}