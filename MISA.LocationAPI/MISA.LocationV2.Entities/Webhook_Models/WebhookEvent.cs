using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace MISA.LocationV2.Webhook.Models
{
    public class WebhookEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string WebhookEventID { get; set; }
        /// <summary>
        /// Uri tới api của web client mà webhook cần gửi thông báo
        /// </summary>
        [Required]
        public string WebhookUri { get; set; }

        /// <summary>
        /// Dữ liệu Webhook gửi đi
        /// </summary>
        public List<WebhookPayload> webhookPayload;
        public WebhookEvent()
        {
        }
    }
}
