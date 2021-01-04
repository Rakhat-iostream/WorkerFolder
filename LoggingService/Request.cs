using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QueueAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LoggingService
{
    public class Request : IDisposable
    {
        private HttpClient client;
        private IConfiguration configuration;

        public Request(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            this.configuration = configuration;
        }
        public void Dispose()
        {
            client.Dispose();
        }

        public async Task<string> SendMessage(string fullPath, FileChangeType type)
        {
            string typeName = Enum.GetName(typeof(FileChangeType), type).ToUpper();
            var message = new
            {
                Type = MessageType.Email,
                JsonContent = $"File named as {fullPath} had {typeName}"
            };
            using (var client = new HttpClient())
            {
          
                var data = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(configuration.GetSection("")["http://localhost:44300/api/queue/add"], data);
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public async Task<string> RespondError(Exception ex)
        {
            var message = new
            {
                MessageType = MessageType.Email,
                JsonContent = $"There is an error : {ex:Message}"
            };
            var data = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(configuration.GetSection("")["http://localhost:44300/api/queue/add"], data);
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
