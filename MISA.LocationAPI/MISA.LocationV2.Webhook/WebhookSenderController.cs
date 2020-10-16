using Microsoft.AspNetCore.Mvc;
using MISA.LocationV2.Webhook.Common;
using MISA.LocationV2.Webhook.Interface;
using MISA.LocationV2.Webhook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using MISA.LocationV2.Webhook.Properties;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Threading;

namespace MISA.LocationV2.Webhook
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookSenderController : IWebhookSender
    {
        private readonly IMongoDatabase _database;
        private readonly IConfiguration _configuration;
        public static IConnection _connection;
        public static IModel _model;

        public WebhookSenderController(IMongoDatabase database, IConfiguration configuration)
        {
            _database = database;
            _configuration = configuration;
            //Khởi tạo connect tới RabbitMQ
            if (_connection == null)
            {

                var uri = _configuration["rabbitmq:connectString"];

                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(uri),
                    DispatchConsumersAsync = true
                };
                _connection = factory.CreateConnection();

            }
            else
            {
                if (!_connection.IsOpen)
                {
                    {
                        var uri = _configuration["rabbitmq:connectString"];
                        var factory = new ConnectionFactory()
                        {
                            Uri = new Uri(uri),
                            DispatchConsumersAsync = true
                        };
                        _connection = factory.CreateConnection();
                    }
                }
            }

        }

        /// <summary>
        /// Xử lý tạo Event từ Queue và Uri để chuẩn bị gửi Webhook Event
        /// </summary>
        /// Create by bvbao (16/9/2020)
        [HttpPost]
        [Route("webhooks")]
        public async Task SendWebhookAsync()
        {
            var sendStorage = GetWaitSendingQueue();

            // Lấy toàn bộ các WebhookSubscription từ nguồn lưu trữ
            var webhookSubscriptions = getAllWebhookSubscription();

            //Nếu không có Uri đăng ký nào thì thôi
            if (sendStorage.Count <= 0 || webhookSubscriptions.Count <= 0)
            {
                Console.WriteLine("Check storage length");
                return;
            }

            //Khởi tạo WebhookEvent với từng WebhookUri cùng data
            foreach (var webhookSubscription in webhookSubscriptions)
            {
                WebhookEvent webhookEvent = new WebhookEntitiesDefine().CreateWebhookEvent(sendStorage, webhookSubscription.WebhookUri);

                //Gửi dữ liệu sang web client
                await PublishAsync(webhookEvent, 0);
            }
        }

        /// <summary>
        /// Tiến hành gửi lại các Event đã gửi lỗi trước đó trong Queue Error
        /// </summary>
        /// Created by bvbao (16/9/2020)
        /// Modified by bvbao (2/10/2020)
        [HttpPost]
        [Route("WebhookRetry")]
        public async Task WebhookSendRetryAsync()
        {
            var listSendError = GetSendErrorQueue();

            if (listSendError.Count <= 0)
            {
                Console.WriteLine("Check error queue length");
                return;
            }

            foreach (var webhookSendError in listSendError)
            {
                await PublishAsync(webhookSendError.webhookEvent, webhookSendError.NumberSendError);
            }
        }

        /// <summary>
        /// Xử lý tạo các Entities, hứng các dữ liệu thay đổi, lưu trữ WebhookEvent vào Queue chờ  
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu cho data thay đổi</typeparam>
        /// <param name="action">Hành động xảy ra với địa chỉ (Thêm, Sửa, Xóa)</param>
        /// <param name="data">Dữ liệu thay đổi</param>
        /// Created by bvbao (13/8/2020)
        ///  Modified by bvbao (2/10/2020)
        public void WebhookStorage<T>(string action, T data)
        {
            //Khởi tạo payload từ dữ liệu địa chỉ thay đổi
            var payload = new WebhookEntitiesDefine().CreateWebhookPayload(data, action);
            //Kết nối đến RabbitMQ
            var queueName = _configuration["rabbitmq:queueWait"];
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queueName, true, false, false, null);
                //Gửi payload lên Queue chờ trên RabbitMQ
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
                channel.BasicPublish("", queueName, null, body);
            }
        }


        /// <summary>
        /// Phương thức gửi thông báo webhook sang web-client thông qua webhookUri
        /// </summary>
        /// <param name="webhookEvent">Dữ liệu Event thay đổi cẩn gửi</param>
        /// <param name="numError">Số lần gửi lỗi của Event này. Bằng 0 nếu gửi lần đầu</param>
        /// <returns>Trả về response khi gửi sang web-client</returns>
        /// Created by bvbao (13/8/2020)
        ///  Modified by bvbao (2/10/2020)
        private async Task PublishAsync(WebhookEvent webhookEvent, int numError)
        {
            bool isSucceed = false;
            HttpStatusCode? responseStatusCode = null;
            string responseContent = Resources.webhookSendFail;

            try
            {
                //Gửi request đến web-client và đợi response để xử lý
                var response = await new SendHttpRequest().SendHttpReq(webhookEvent);
                isSucceed = response.isSucceed;
                responseStatusCode = response.statusCode;
                responseContent = response.content;
            }
            catch (TaskCanceledException)//since we run it background and never send cancellation token TaskCanceledException means request timeout
            {
                responseStatusCode = HttpStatusCode.RequestTimeout;
                responseContent = Resources.RequestTimeout;
            }
            catch (HttpRequestException e)//something wrong happened on request. we can show them to users.
            {
                responseContent = e.Message;
            }
            catch (Exception e)// an internal error occurred. do not show it to users. just log it.
            {
                responseContent = e.ToString();
            }
            finally
            {
                if (!isSucceed) //Gửi không thành công thì lưu trữ và Queue eror để xử lý gửi lại sau
                {
                    var maxSendError = int.Parse(_configuration["webhook:maxSendError"]);
                    //Nếu gửi lỗi quá nhiều thì đẩy vào database để xử lý gửi sau bằng tay
                    if (numError >= maxSendError)
                    {
                        //Nếu đẩy vào database lỗi thì lại đưa event vào Queue, nếu thành công thì thôi
                        if (SaveEventErrorToDB(webhookEvent) == null)
                        {
                            var queueName = _configuration["rabbitmq:queueError"];
                            //Khởi tạo WebhookSendAttempt từ dữ liệu địa chỉ thay đổi
                            var attempt = new WebhookSendAttempt()
                            {
                                webhookEvent = webhookEvent,
                                ResponseStatusCode = responseStatusCode,
                                Response = responseContent,
                                NumberSendError = numError + 1
                            };
                            //Kết nối đến RabbitMQ
                            using (var channel = _connection.CreateModel())
                            {
                                channel.QueueDeclare(queueName, true, false, false, null);
                                //Gửi WebhookSendAttempt lên Queue lỗi trên RabbitMQ
                                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(attempt));
                                channel.BasicPublish("", queueName, null, body);
                            }
                        }
                    }
                    else
                    {
                        var queueName = _configuration["rabbitmq:queueError"];
                        //Khởi tạo WebhookSendAttempt từ dữ liệu địa chỉ thay đổi
                        var attempt = new WebhookSendAttempt()
                        {
                            webhookEvent = webhookEvent,
                            ResponseStatusCode = responseStatusCode,
                            Response = responseContent,
                            NumberSendError = numError + 1
                        };
                        //Kết nối đến RabbitMQ
                        using (var channel = _connection.CreateModel())
                        {
                            channel.QueueDeclare(queueName, true, false, false, null);
                            //Gửi WebhookSendAttempt lên Queue lỗi trên RabbitMQ
                            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(attempt));
                            channel.BasicPublish("", queueName, null, body);
                        }
                    }
                }
            }
        }

        #region Common Function
        /// <summary>
        /// Lấy toàn bộ danh sách các webhook đã đăng ký còn hoạt động trong mongoDB
        /// </summary>
        /// <returns></returns>
        /// Created by bvbao (8/9/2020)
        private List<WebhookSubscription> getAllWebhookSubscription()
        {
            var _collection = _database.GetCollection<WebhookSubscription>(Resources.collectionSubscription);
            return _collection.Find<WebhookSubscription>(uri => uri.IsActive == true).ToList();
        }

        /// <summary>
        /// Lưu lại Event lỗi vào database mongoDB sau khi gửi không thành công quá nhiều lần
        /// </summary>
        /// <param name="webhookEvent">Event lỗi</param>
        /// <returns></returns>
        /// Created by bvbao(10/9/2020)
        public async Task<WebhookEvent> SaveEventErrorToDB(WebhookEvent webhookEvent)
        {
            var _collection = _database.GetCollection<WebhookEvent>(Resources.collectionEventError);
            await _collection.InsertOneAsync(webhookEvent);
            return _collection.AsQueryable().Where(x => x.WebhookEventID == webhookEvent.WebhookEventID).FirstOrDefault();
        }

        /// <summary>
        /// Lấy danh sách dữ liệu cần gửi trong Queue chờ trên RabbitMQ
        /// </summary>
        /// <returns></returns>
        /// Created by bvbao(2/10/2020)
        private List<WebhookPayload> GetWaitSendingQueue()
        {
            var sendStorage = new List<WebhookPayload>();
            //Kết nối đến RabbitMQ và Lấy dữ liệu trong Queue chờ
            var queueName = _configuration["rabbitmq:queueWait"];
            using (var channel = _connection.CreateModel())
            {
                var numMeg = channel.QueueDeclare(queueName, true, false, false, null).MessageCount;
                var payload = new WebhookPayload();
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    payload = JsonConvert.DeserializeObject<WebhookPayload>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                    sendStorage.Add(payload);
                };
                string consumerTag = channel.BasicConsume(queueName, true, consumer);
            }

            return sendStorage;
        }

        /// <summary>
        /// Lấy danh sách dữ liệu gửi lỗi trong Queue lỗi trên RabbitMQ
        /// </summary>
        /// <returns></returns>
        /// Created by bvbao(2/10/2020)
        private List<WebhookSendAttempt> GetSendErrorQueue()
        {
            var listSendError = new List<WebhookSendAttempt>();
            //Kết nối đến RabbitMQ và Lấy dữ liệu trong Queue lỗi
            var queueName = _configuration["rabbitmq:queueError"];
            using (var channel = _connection.CreateModel())
            {
                var numMeg = channel.QueueDeclare(queueName, true, false, false, null).MessageCount;
                var sendAttempt = new WebhookSendAttempt();
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    sendAttempt = JsonConvert.DeserializeObject<WebhookSendAttempt>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                    listSendError.Add(sendAttempt);
                };
                string consumerTag = channel.BasicConsume(queueName, true, consumer);
            }
            return listSendError;
        }
        #endregion
    }
}
