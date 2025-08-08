using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orchestrator.Domain.Entities;

namespace Orchestrator.Infrastructure.Persistence
{
    /// <summary>
    /// The database context for the Orchestrator application, acting as the main
    /// gateway for data access with Entity Framework Core.
    /// </summary>
    public class OrchestratorDbContext: DbContext
    {
        // This constructor is essential. It allows the dependency injection container
        // in our API project to pass in configuration, like the database connection string.
        public OrchestratorDbContext(DbContextOptions<OrchestratorDbContext> options)
            : base(options)
        {
        }

        public DbSet<App> Apps { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ClientHeartbeat> ClientHeartbeats { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ConfigAsset> ConfigAssets { get; set; }
        public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
    }
}
