using QueueAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageType = QueueAPI.Models.MessageType;

namespace QueueAPI.Repositories.Interfaces
{
   public  interface IQueueRepository
    {
        Task<bool> AddMessage(Message message);
        Task<bool> SetHandled(Guid messageId);
        Task<Message> GetUnhandledMessage(MessageType type);
        
    }
}
