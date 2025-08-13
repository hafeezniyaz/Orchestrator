using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.ApiClients.Models
{
    public class CreateApiClientResponse
    {
        public Guid Id { get; set; }
        public required string ClientName { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; } // IMPORTANT: This is sent only once upon creation.
        public required List<string> RoleNames { get; set; }
        public bool IsActive { get; set; }
    }
}
