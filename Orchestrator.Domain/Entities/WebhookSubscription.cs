using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// represents client subscription to a specfic server-side events
    /// </summary>
    [Table("TMUSWebhookSub")]
    public class WebhookSubscription: AuditableEntity
    {
        public required string ClientId { get; set; }
        public required string  CallbackUrl { get; set; }
        public required string EventType { get; set; }
    }
}
