using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Orchestrator.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Persistance
{
    /// <summary>
    /// this will be used by entity frameword to inject context while design time since some of the context can only be provided at runtime such as httpcontext
    /// </summary>
    public class OrchestratorDbContextFactory : IDesignTimeDbContextFactory<OrchestratorDbContext>
    {

        public OrchestratorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrchestratorDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OrchestratorDb;Integrated Security=True;TrustServerCertificate=True;");

            var dummyUser = new DummyCurrentContextUserProvider();
            return new OrchestratorDbContext(optionsBuilder.Options,dummyUser);
        }
    }

    internal class DummyCurrentContextUserProvider : ICurrentUserProvider
    {
        public string? UserName => "migration_user";
    }
}
