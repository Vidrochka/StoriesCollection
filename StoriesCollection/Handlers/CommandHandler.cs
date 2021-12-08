using Microsoft.Extensions.Logging;
using StoriesCollection.Db.Repository;
using StoriesCollection.Helpers;
using StoriesCollection.Models;
using StoriesCollection.Telegram;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

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
            switch(command)
            {
                case "/stories" or "/start":
                    {
                        await _messageService.SendStartMessage(chatId);
                        break;
                    }
                case "/admin":
                    {
                        var message = $"Чтобы добавить историю отправьте сообщение в формате \r\n\t " +
                                $"``` admin;story;add;название истории ``` \r\n\t\t " +
                                $"\\* В ответ прийдет id истории\r\n " +
                            $"Чтобы добавить часть истории отправьте сообщение в формате \r\n\t " +
                                $"``` admin;story part;add;id истории;текст части истории ``` \r\n\t\t " +
                                $"\\* В ответ прийдет id части истории\r\n\t\t " +
                                $"\\* Первая созданная часть истории становится первой частью истории\r\n\t\t " +
                            $"Чтобы добавить переход между частями истории отправьте сообщение в формате \r\n\t " +
                                $"``` admin;button;add;id части истории откуда переход;id части истории куда переход;текст кнопки ``` \r\n\t\t " +
                                $"\\* Кнопка завершения истории должна оставить пустым поле 'id части истории куда переход'";
                        await _messageService.SendSimpleMessage(chatId, message, ParseMode.MarkdownV2);
                        break;
                    }
                default:
                    {
                        _logger.LogInformation($"Нераспознанная комманда: [{command}]");
                        break;
                    }
            }
        }
    }
}
