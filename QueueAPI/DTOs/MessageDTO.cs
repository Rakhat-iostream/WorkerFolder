using QueueAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueAPI.DTOs
{
    public class MessageDTO
    {
        public Guid Id { get; set; } 
        public bool Handled { get; set; }
        public MessageType Type { get; set; }
        public string JsonContent { get; set; } 
    }
}
