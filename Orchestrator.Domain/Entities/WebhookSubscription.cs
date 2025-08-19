using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents a client's subscription to a specific server-side event.
    /// </summary>
    [Table("TMUSWebhookSub")]
    public class WebhookSubscription: AuditableEntity
    {
        public required string ClientId { get; set; }
        public required string CallbackUrl { get; set; }
        public required string EventType { get; set; } // e.g., "package.published"
    }
}
