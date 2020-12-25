using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmailService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceWorkerFolder
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailSender _sender;
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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                watcher.EnableRaisingEvents = true;
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
