using Orchestrator.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Orchestrator.Infrastructure.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

            public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        //public string? UserId => _httpContextAccessor.HttpContext?.User?.Claims
        //    .FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;

        // public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity.Name;
        public string? UserName => _httpContextAccessor.HttpContext?.User?.Claims
           .FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
    }
}
