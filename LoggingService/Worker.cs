using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LoggingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration configuration;
        private FileSystemWatcher watcher;
        private Request messages;
        private readonly string path = @"C:\Users\Rakhat\Desktop\MyFolder";


        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            messages = new Request(new HttpClient(), configuration);
            
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
                watcher.EnableRaisingEvents = true;
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
