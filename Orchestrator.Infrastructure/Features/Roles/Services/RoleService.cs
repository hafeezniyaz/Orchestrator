using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.Roles.Interfaces;
using Orchestrator.Application.Features.Roles.Models;
using Orchestrator.Infrastructure.Persistence;

namespace Orchestrator.Infrastructure.Features.Roles.Services
{
    public class RoleService : IRoleService
    {
        private readonly OrchestratorDbContext _context;

        public RoleService(OrchestratorDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RoleDto>> GetRolesAsync(string? name, int skip, int top)
        {
            var query = _context.Roles.AsNoTracking();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(r => r.Name)
                .Skip(skip)
                .Take(top)
                .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
                .ToListAsync();

            return new PagedResult<RoleDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }
}