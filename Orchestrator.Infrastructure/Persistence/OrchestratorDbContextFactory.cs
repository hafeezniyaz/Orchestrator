using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Orchestrator.Application.Common.Interfaces;

namespace Orchestrator.Infrastructure.Persistence
{
    /// <summary>
    /// This factory is used by the EF Core command-line tools (e.g., for Add-Migration)
    /// to create a DbContext instance at design time. It bypasses the application's
    /// main service provider, avoiding issues with services that require an active HTTP context.
    /// </summary>
    public class OrchestratorDbContextFactory : IDesignTimeDbContextFactory<OrchestratorDbContext>
    {
        public OrchestratorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrchestratorDbContext>();

            // We can hard-code the development connection string here because this class
            // is only used by developers' command-line tools, not by the running application.
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=OrchestratorDb;Integrated Security=True;TrustServerCertificate=True;");

            var dummyUserProvider = new DummyCurrentUserProvider();

            return new OrchestratorDbContext(optionsBuilder.Options, dummyUserProvider);
        }
    }
    internal class DummyCurrentUserProvider : ICurrentUserProvider
    {
        public string? UserId => "migration_user";
        public string? UserName => "migration_user";
    }
} 