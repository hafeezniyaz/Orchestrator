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

        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleMapping> RoleMappings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // It's important to call the base method
            var seedDate = new DateTime(2025, 8, 12, 0, 0, 0, DateTimeKind.Utc);

            // Seed Roles with fixed IDs for stable foreign key relationships
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("0d52554b-b12a-4c87-9352-3781e1054948"), Name = "Administrator", CreatedAt = seedDate },
                new Role { Id = Guid.Parse("e3b3b4a2-1e1b-4c1a-9b4e-8a3d11b02b3a"), Name = "Publisher", CreatedAt = seedDate },
                new Role { Id = Guid.Parse("a1b2c3d4-5678-90ab-cdef-1234567890ab"), Name = "Viewer", CreatedAt = seedDate },
                new Role { Id = Guid.Parse("f5e4d3c2-b1a0-9876-5432-10fedcba9876"), Name = "User", CreatedAt = seedDate },
                new Role { Id = Guid.Parse("71cb4b4c-e3f6-48b1-b800-cbf20acbf1ab"), Name = "Support", CreatedAt = seedDate }
            );
        }
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
