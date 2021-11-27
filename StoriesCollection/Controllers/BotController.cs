using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoriesCollection.Handlers;
using StoriesCollection.Models;
using StoriesCollection.Telegram.Helpers;
using StoriesCollection.Telegram.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace StoriesCollection.Controllers
{
    [Route("api/Bot")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly ILogger<BotController> _logger;
        private readonly CallbackHandler _callbackHandler;
        private readonly CommandHandler _commandHandler;
        private readonly MessageHandler _messageHandler;

        public BotController(ILogger<BotController> logger, CallbackHandler callbackHandler, CommandHandler commandHandler, MessageHandler messageHandler)
        {
            _logger = logger;
            _callbackHandler = callbackHandler;
            _commandHandler = commandHandler;
            _messageHandler = messageHandler;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Ok");
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            switch (EventClassifier.CheckEvent(update))
            {
                case EventType.Message:
                    {
                        await _messageHandler.Handle(update.Message.Chat.Id, update.Message.Text);
                        break;
                    }
                case EventType.Command:
                    {
                        await _commandHandler.Handle(update.Message.Chat.Id, update.Message.Text);
                        break;
                    }
                case EventType.Callback:
                    {
                        await _callbackHandler.Handle(update.CallbackQuery.From.Id, update.CallbackQuery.Data);
                        break;
                    }
                case null:
                    {
                        _logger.LogInformation("Нераспознанный типо сообщения");
                        break;
                    }
            }

            return Ok();
        }
    }
}
