using MISA.LocationV2.Webhook.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.LocationV2.Webhook.Interface
{
    public interface IWebhookSender
    {
        /// <summary>
        /// Khởi tạo và lưu các giá trị dữ liệu thay đổi lưu trữ vào Queue trong RabbitMQ
        /// </summary>
        /// <typeparam name="T">Location</typeparam>
        /// <param name="action">Hành động Thêm/sửa/xóa</param>
        /// <param name="data">Giá trị dữ liệu thay đổi</param>
        /// created by bvbao (14/10/2020)
        public void WebhookStorage<T>(string action, T data);
        /// <summary>
        /// API lấy dữ liệu từ Queue chờ trên RabbitMQ gửi cho các bên Client
        /// </summary>
        /// <returns></returns>
        /// created by bvbao (14/10/2020)
        public Task SendWebhookAsync();
        /// <summary>
        ///  API lấy dữ liệu từ Queue lỗi trên RabbitMQ gửi lại cho các Client
        /// </summary>
        /// <returns></returns>
        ///  created by bvbao (14/10/2020)
        public Task WebhookSendRetryAsync();
    }
}
