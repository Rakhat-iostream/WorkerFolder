using System;

namespace QueueAPI.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public bool Handled { get; set; } 
        public MessageType Type { get; set; }
        public string JsonContent { get; set; }
    }
}
