using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Domain.Entities;

namespace Orchestrator.Infrastructure.Persistence
{
    /// <summary>
    /// The database context for the Orchestrator application, acting as the main
    /// gateway for data access with Entity Framework Core.
    /// </summary>
    public class OrchestratorDbContext: DbContext
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        // This constructor is essential. It allows the dependency injection container
        // in our API project to pass in configuration, like the database connection string.
        public OrchestratorDbContext(DbContextOptions<OrchestratorDbContext> options, ICurrentUserProvider currentUserProvider)
            : base(options)
        {
            _currentUserProvider = currentUserProvider;
        }

        public DbSet<App> Apps { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ClientHeartbeat> ClientHeartbeats { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ConfigAsset> ConfigAssets { get; set; }
        public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
        public DbSet<ApiClient> ApiClients { get; set; }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userName = _currentUserProvider.UserName;
            var now = DateTime.UtcNow;

            // Find all entities that inherit from AuditableEntity and are being added or modified
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = userName;
                        entry.Entity.CreatedAt = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = userName;
                        entry.Entity.ModifiedAt = now;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
