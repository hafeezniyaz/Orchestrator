using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.ApiClient.Models
{
    public class ApiClientDto
    {
        public required string ClientName { get; set; }
        public required Guid ClientId { get; set; }

        public required IEnumerable<string> Roles { get; set; }

        public bool IsActive { get; set; }

    }
}
