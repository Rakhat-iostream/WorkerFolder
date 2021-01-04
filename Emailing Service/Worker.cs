using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueueAPI.DTOs;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EmailingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IEmailSender _sender;
        private FileSystemWatcher watcher;
        private readonly string path = @"C:\Users\Rakhat\Desktop\MyFolder";
        public Worker(ILogger<Worker> logger, IEmailSender sender)
        {
            _logger = logger;
            _sender = sender;
           
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
       
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            ChangesOccured();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
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
                            await _sender.SendEmail(message);
                            await client.GetAsync("http://localhost:44300/api/queue/handled" + email.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                    watcher.EnableRaisingEvents = true;
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("Changed file" + e.Name, DateTimeOffset.Now);
            var message = new Message(new string[] { "1907aitu@gmail.com" }, "You file had some changes", $"{e.Name} have changed", e.FullPath);
            _sender.SendEmail(message);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("Created a new file at", DateTimeOffset.Now);
            var message = new Message(new string[] { "1907aitu@gmail.com" }, "You have created a new file", $"{e.Name} have been created", e.FullPath);
            _sender.SendEmail(message);
        }

        private void OnRenamed(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"Your file renamed to " + e.Name, DateTimeOffset.Now);
            var message = new Message(new string[] { "1907aitu@gmail.com" }, "Your file name have been renamed", $"Your file have been renamed to {e.Name}", e.FullPath);
            _sender.SendEmail(message);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            _logger.LogWarning("Your file have been deleted" + e.Name, DateTimeOffset.Now);
            var message = new Message(new string[] { "1907aitu@gmail.com" }, "Your file have been deleted", $"File {e.Name} have been deleted", e.FullPath);
            _sender.SendEmail(message);
        }

        private void OnErrored(object sender, ErrorEventArgs e)
        {
            var exception = e.GetException();
            _logger.LogCritical(exception.Message + '\n' + exception.StackTrace);
            var message = new Message(new string[] { "1907aitu@gmail.com" }, "Error, while dealing with file", e.GetException().Message, path);
            _sender.SendEmail(message);
        }


        private void ChangesOccured()
        {
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Renamed += OnRenamed;
            watcher.Deleted += OnDeleted;
            watcher.Error += OnErrored;
        }
    }
}
