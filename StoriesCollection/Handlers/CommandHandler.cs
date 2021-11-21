using Microsoft.Extensions.Logging;
using StoriesCollection.Db.Repository;
using StoriesCollection.Helpers;
using StoriesCollection.Models;
using StoriesCollection.Telegram;
using System.Linq;
using System.Threading.Tasks;

namespace StoriesCollection.Handlers
{
    public class CommandHandler
    {
        private readonly ILogger<CallbackHandler> _logger;
        private readonly MessageService _messageService;

        public CommandHandler(ILogger<CallbackHandler> logger, MessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        public async Task Handle(long chatId, string? command)
        {
            if (command != "/stories" && command != "/start")
            {
                _logger.LogInformation($"Нераспознанная комманда: [{command}]");
                return;
            }

            await _messageService.SendStartMessage(chatId);
        }
    }
}
