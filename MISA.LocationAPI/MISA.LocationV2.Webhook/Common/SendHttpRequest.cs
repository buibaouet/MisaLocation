using MISA.LocationV2.Webhook.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MISA.LocationV2.Webhook
{
    public class SendHttpRequest
    {
        /// <summary>
        /// Gửi request tới bên web-client theo Uri
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// Created by bvbao (13/8/2020)
        public async Task<(bool isSucceed, HttpStatusCode statusCode, string content)> SendHttpReq(WebhookEvent webhookEvent)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, webhookEvent.WebhookUri);//create webhook request using parameters

            request.Content = new StringContent(JsonConvert.SerializeObject(webhookEvent.webhookPayload), Encoding.UTF8, "application/json");
            using (var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(60)//make sure you define timeout
            })
            {
                var response = await client.SendAsync(request);

                var isSucceed = response.IsSuccessStatusCode;
                var statusCode = response.StatusCode;
                var content = await response.Content.ReadAsStringAsync();

                return (isSucceed, statusCode, content);
            }
        }
    }
}
