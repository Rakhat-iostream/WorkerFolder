using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueueAPI.DTOs;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EmailingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IEmailSender sender;
        private HttpClient client;
        private IConfiguration configuration;
        public Worker(ILogger<Worker> logger, IEmailSender sender, IConfiguration configuration)
        {
            _logger = logger;
            this.sender = sender;
            this.configuration = configuration;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("http://localhost:44300/api/queue/retrieve/email");
                    string result = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var email = JsonConvert.DeserializeObject<MessageDTO>(result);
                        if (email != null)
                        {
                            var message = new Message(new string[] { "1907aitu@gmail.com" }, "Email Service", email.JsonContent, "");
                            await sender.SendEmail(message);
                            await client.GetAsync("http://localhost:44300/api/queue/handled" + email.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
