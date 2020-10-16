using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.LocationV2.Webhook.Models
{
    public class WebhookPayload
    {
        public Guid Id { get; set; }

        public string ActionType { get; set; }

        public string Description { get; set; }

        public dynamic Data { get; set; }

        public DateTime ActionTimeUtc { get; set; }

        public WebhookPayload()
        {
            Id = Guid.NewGuid();
            ActionTimeUtc = DateTime.UtcNow;
        }
    }
}