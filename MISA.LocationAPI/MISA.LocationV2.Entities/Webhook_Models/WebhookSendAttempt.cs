using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MISA.LocationV2.Webhook.Models
{
    public class WebhookSendAttempt
    {
        /// <summary>
        /// Thông tin về webhook
        /// </summary>
        public WebhookEvent webhookEvent;

        /// <summary>
        /// Webhook response content that webhook endpoint send back
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Webhook response status code that webhook endpoint send back
        /// </summary>
        public HttpStatusCode? ResponseStatusCode { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Số lần đã gửi bị lỗi
        /// </summary>
        public int NumberSendError { get; set; }

        public WebhookSendAttempt()
        {
            CreationTime = DateTime.Now;
        }
    }
}
