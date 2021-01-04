using Microsoft.EntityFrameworkCore;
using QueueAPI.Data;
using QueueAPI.Models;
using QueueAPI.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueAPI.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private DataContext context;

        public QueueRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddMessage(Message message)
        {
            context.MessageQueue.Add(message);
            return await context.SaveChangesAsync() > 0;
        }

        public async  Task<Message> GetUnhandledMessage(MessageType type)
        {
            return await context.MessageQueue.FirstOrDefaultAsync(x => x.Handled == false && x.Type.Equals(type));
        }


        public async Task<bool> SetHandled(Guid messageId)
        {
            var message = context.MessageQueue.Where(x => x.Id == messageId).FirstOrDefault();
            if (message == null)
                return false;
            message.Handled = true;
            return (await context.SaveChangesAsync()) > 0;
        }


    }
}
