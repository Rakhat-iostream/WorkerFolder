using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailingService
{
   public interface IEmailSender
    {
        Task SendEmail(Message message);
    }
}
