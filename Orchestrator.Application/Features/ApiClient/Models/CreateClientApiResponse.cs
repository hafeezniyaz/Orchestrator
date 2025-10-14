using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.ApiClient.Models
{
    public class CreateClientApiResponse
    {
        public Guid Id { get; set; }
        public required string ClientName { get; set; }
        public required string  ClientSecret { get; set; }

        public required Guid ClientId { get; set; }

        public required IEnumerable<string> Roles { get; set; }


        public bool IsActive { get; set; }
    }
}
