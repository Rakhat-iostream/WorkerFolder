using Microsoft.AspNetCore.Mvc;
using QueueAPI.DTOs;
using QueueAPI.Models;
using QueueAPI.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using MessageType = QueueAPI.Models.MessageType;

namespace QueueAPI.Controllers
{
    [Route("api/queue/")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly IQueueRepository messages;

        public QueueController(IQueueRepository messages)
        {
            this.messages = messages;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddMessageAsync(MessageDTO m)
        {
            var message = new Message
            {
                Type = m.Type,
                JsonContent = m.JsonContent
            };
            if (await messages.AddMessage(message)) return Ok("Added a  new message");
            return BadRequest("It's an error to sending message");
        }
        [HttpGet("handled/{message_id}")]
        public async Task<IActionResult> HandleMessageByIdAsync(Guid id)
        {
            if (await messages.SetHandled(id)) return Ok($"Handled message by id {id}");

            return BadRequest("Failed to handle.");
        }
        [HttpGet("retrieve/email")]
        public async Task<MessageDTO> GetMessagesByEmailAsync()
        {
            var message = await messages.GetUnhandledMessage(MessageType.Email);
            if (message == null) return null;
            return new MessageDTO
            {
                Id = message.Id,
                Handled = message.Handled,
                JsonContent = message.JsonContent,
                Type = message.Type
            };
        }
        [HttpGet("retrieve/log")]
        public async Task<MessageDTO> GetMessagesByLogAsync()
        {
            var message = await messages.GetUnhandledMessage(MessageType.Log);
            if (message == null) return null;
            return new MessageDTO
            {
                Id = message.Id,
                Handled = message.Handled,
                JsonContent = message.JsonContent,
                Type = message.Type
            };
        }
    }
}