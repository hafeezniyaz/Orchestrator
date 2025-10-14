using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.ApiClient.Models
{
    public class UpdateApiClientRoleRequest
    {
        [Required]
        public required IEnumerable<string> Roles { get; set; }
    }
}
