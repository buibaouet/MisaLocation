using MISA.LocationAPI.Models;
using MISA.LocationV2.Webhook.Models;
using MISA.LocationV2.Webhook.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.LocationV2.Webhook.Common
{
    public class WebhookEntitiesDefine
    {
        /// <summary>
        /// Khởi tạo dữ liệu cần gửi cho phía client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location">Dữ liệu về location có sự thay đổi</param>
        /// <param name="action">Hành động thay đổi với location (thêm, sửa, xóa)</param>
        /// <returns></returns>
        /// Creadby bvbao(29/9/2020)
        public WebhookPayload CreateWebhookPayload<T>(T location, string action)
        {
            var message = (action == "INSERT") ? Resources.insertMeg:
                (action == "UPDATE") ? Resources.updateMeg : Resources.deleteMeg;
            return new WebhookPayload()
            {
                ActionType = action,
                Description = message,
                Data = location
            };
        }

        /// <summary>
        /// Khởi tạo một sự kiện webhook mới
        /// </summary>
        /// <param name="webhookPayload">Dữ liệu của sự kiện Webhook</param>
        /// <param name="Uri">Uri của phía client đăng ký webhook</param>
        /// <returns></returns>
        /// Createdby bvbao(29/9/2020)
        public WebhookEvent CreateWebhookEvent(List<WebhookPayload> webhookPayload, string Uri)
        {
            return new WebhookEvent()
            {
                WebhookUri = Uri,
                webhookPayload = webhookPayload
            };
        }
    }
}
