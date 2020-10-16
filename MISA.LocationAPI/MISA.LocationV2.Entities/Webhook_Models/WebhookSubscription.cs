using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MISA.LocationV2.Webhook.Models
{
    public class WebhookSubscription
    {
        /// <summary>
        /// ID 
        /// </summary>
        /// [BsonId]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string WebhookSubscriptionId { get; set; }

        /// <summary>
        /// Subscription webhook endpoint
        /// </summary>
        public string WebhookUri { get; set; }

        /// <summary>
        /// Website name subscription webhook endpoint
        /// </summary>
        public string WebhookName { get; set; }

        /// <summary>
        /// Webhook secret
        /// </summary>
        public string Secret { get; set; } = "";

        /// <summary>
        /// Is subscription active
        /// </summary>
        public bool IsActive { get; set; }

        public WebhookSubscription()
        {
            IsActive = true;
        }
    }
}
