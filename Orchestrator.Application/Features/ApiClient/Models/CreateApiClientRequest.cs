using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.ApiClient.Models
{
    public class CreateApiClientRequest
    {
        [Required]
        [MaxLength(100)]
        public required string ClientName { get; set; }

        [Required]
        public required IEnumerable<string> Roles { get; set; }
    }
}
