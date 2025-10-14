using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Orchestrator.Application.Common.Interfaces;
using Orchestrator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Persistance
{
    /// <summary>
    /// the database context for orchestrator application 
    /// </summary>
    public class OrchestratorDbContext : DbContext
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        public OrchestratorDbContext(DbContextOptions<OrchestratorDbContext> options, ICurrentUserProvider currentUserProvider)
            :base(options)
        {
            _currentUserProvider = currentUserProvider;
        }

        public DbSet<App> Apps { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Log> Logs  { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }      
        public DbSet<ClientHeartbeat> ClientHeartbeats { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ConfigAsset> ConfigAssets { get; set; }
        public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
        public DbSet<ApiClient> ApiClients { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleMapping> RoleMappings { get; set; }

        public DbSet<Credential> Credentials{ get; set; }

        //for role seeding in code first approcah 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //App->Config Relation
            modelBuilder.Entity<App>()
                .HasMany<Config>()
                .WithOne()
                .HasForeignKey(c => c.AppId)
                .OnDelete(DeleteBehavior.Cascade);

            //Config->Asset Relation
            modelBuilder.Entity<Config>()
                .HasMany<ConfigAsset>()
                .WithOne()
                .HasForeignKey(a => a.ConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            //App->Package Relation
            modelBuilder.Entity<App>()
                .HasMany<Package>()
                .WithOne()
                .HasForeignKey(p => p.AppId)
                .OnDelete(DeleteBehavior.Cascade);

            //ApiClient->RoleMapping Relation
            modelBuilder.Entity<ApiClient>()
                .HasMany<RoleMapping>()
                .WithOne()
                .HasPrincipalKey(rm => rm.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            //Role->RoleMapping , the default behavior by EF is Restrict hower below is for making it obvious
            modelBuilder.Entity<Role>()
                .HasMany<RoleMapping>()
                .WithOne()
                .HasForeignKey(rm => rm.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            //App->Credentials Mapping
            modelBuilder.Entity<App>()
                .HasMany<Credential>()
                .WithOne()
                .HasForeignKey(c => c.AppId)
                .OnDelete(DeleteBehavior.Cascade);

            //Credential -> unique entry of (app,name) combination
            modelBuilder.Entity<Credential>()
                .HasIndex(c => new { c.Name, c.AppId })
                .IsUnique();


            //for dev
            //var createdAt = new DateTime(2022, 8, 12, 0, 0, 0, DateTimeKind.Utc);

            //modelBuilder.Entity<Role>().HasData(
            //    new Role { Id = Guid.Parse("a6bc38c5-14e2-45e0-a891-1ce83a2e9c4c"), Name = "Admin", CreatedAt = createdAt },
            //    new Role { Id = Guid.Parse("0d550734-f397-448d-ba0d-3138c5c09366"), Name = "Publish", CreatedAt = createdAt },
            //    new Role { Id = Guid.Parse("79dfebca-e5a3-4540-b92e-c8f86c2a42d1"), Name = "View", CreatedAt = createdAt },
            //    new Role { Id = Guid.Parse("74be2113-0316-4597-9a05-86a777c6b830"), Name = "User", CreatedAt = createdAt },
            //    new Role { Id = Guid.Parse("53f35a3e-0ada-46dd-9772-a730b950edfe"), Name = "Support", CreatedAt = createdAt }
            //    );

        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) //overdie the save change async method to add audiatble fields
        {
            var userName = _currentUserProvider.UserName;
            var now = DateTime.UtcNow;

            //find all the entities which inherit auditable entity and insert the auditable values
            foreach(var entry in ChangeTracker.Entries<AuditableEntity>())
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
