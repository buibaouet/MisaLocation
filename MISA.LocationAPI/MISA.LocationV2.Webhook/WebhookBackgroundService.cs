using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MISA.LocationV2.Webhook.Interface;
using MISA.LocationV2.Webhook.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.LocationV2.Webhook
{
    public class WebhookBackgroundService : IHostedService
    {
        public readonly IWebhookSender webhookSender;
        private readonly IConfiguration _configuration;

        public WebhookBackgroundService(IWebhookSender webhookSender, IConfiguration configuration)
        {
            this.webhookSender = webhookSender;
            _configuration = configuration;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //Delay 1 ngày và gửi webhook sau mỗi một ngày
                await Task.Delay(TimeSpan.FromDays(1));
                await webhookSender.WebhookSendRetryAsync();
                await webhookSender.SendWebhookAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
