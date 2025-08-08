using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Orchestrator.Application.Common.Interfaces;

namespace Orchestrator.Infrastructure.Persistence
{
    public class OrchestratorDbContextFactory : IDesignTimeDbContextFactory<OrchestratorDbContext>
    {
        public OrchestratorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrchestratorDbContext>();
            
            // Use SQLite for migrations since LocalDB is not available
            optionsBuilder.UseSqlite("Data Source=temp.db");
            
            // Create a mock current user provider for design-time
            var mockCurrentUserProvider = new MockCurrentUserProvider();
            
            return new OrchestratorDbContext(optionsBuilder.Options, mockCurrentUserProvider);
        }
    }

    // Simple mock for design-time
    public class MockCurrentUserProvider : ICurrentUserProvider
    {
        public string? UserId => "design-time-user";
        public string? UserName => "design-time-user";
    }
}