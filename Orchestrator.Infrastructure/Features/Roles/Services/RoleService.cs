using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.Roles.Interfaces;
using Orchestrator.Application.Features.Roles.Models;
using Orchestrator.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.Roles.Services
{
    public class RoleService : IRoleService
    {
        private readonly OrchestratorDbContext _context;
        private readonly ILogger<RoleService> _logger;
        public RoleService(OrchestratorDbContext orchestratorDbContext, ILogger<RoleService> logger)
        {
            _context = orchestratorDbContext;
            _logger = logger;
        }
        public async Task<PagedResult<RoleDto>> GetRolesAsync(string? name, int skip = 0, int top = 50)
        {
            var query = _context.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(r => r.Name.Contains(name));

            var totalCount = await query.CountAsync();

            var roles = await query
                .Skip(skip)
                .Take(top)
                .Select(role => new RoleDto { Name = role.Name, RoleId = role.Id })
                .ToListAsync();

            return new PagedResult<RoleDto> { TotalCount = totalCount, Items = roles };
        }
    }
}
