using Microsoft.Extensions.Logging;
using StoriesCollection.Db.Models;
using StoriesCollection.Db.Repository;
using StoriesCollection.Helpers;
using StoriesCollection.Models;
using StoriesCollection.Telegram;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoriesCollection.Handlers
{
    public class CallbackHandler
    {
        private readonly ILogger<CallbackHandler> _logger;
        private readonly MessageService _messageService;
        private readonly TelegramGateway _telegramGateway;
        private readonly StoriesRepository _storiesRepository;

        public CallbackHandler(ILogger<CallbackHandler> logger, MessageService messageService, StoriesRepository storiesRepository, TelegramGateway telegramGateway)
        {
            _logger = logger;
            _messageService = messageService;
            _storiesRepository = storiesRepository;
            _telegramGateway = telegramGateway;
        }

        public async Task Handle(long chatId, string? callback)
        {
            try
            {
                if(string.IsNullOrEmpty(callback)) throw new Exception($"Инвалидный callback chatId:[{chatId}]");

                switch (ButtonFactory.GetButton(callback))
                {
                    case StartStoryButton startButton:
                        {
                            if (startButton.StartMessageId is not null) await _telegramGateway.DeleteMessage(chatId, startButton.StartMessageId.Value);

                            var storyInfo = await _storiesRepository.GetStoryInfo(startButton.StoryId);
                            await _messageService.SendSimpleMessage(chatId, $"------<b><i>{storyInfo?.Name}</i></b>------");

                            if (storyInfo?.FirstStoryPartId is null) throw new Exception("Ожидалось id первой части истории");

                            var firstStoryPart = await _storiesRepository.GetStoryPartWithButtons(storyInfo.FirstStoryPartId.Value);

                            if (firstStoryPart is null) throw new Exception($"Не найдена части истории [{storyInfo.FirstStoryPartId.Value}]");

                            await _messageService.SendStoryPart(chatId, firstStoryPart);

                            break;
                        }
                    case EndStoryButton endButton:
                        {
                            var button = await _storiesRepository.GetButton(endButton.ButtonId);

                            if (button is not null)
                            {
                                await _messageService.SendSimpleMessage(chatId, $"<i>{button.Text}</i>");

                                var currentPart = await _storiesRepository.GetStoryPartWithButtons(button.SourceStoryPartId);
                                if (currentPart is not null && endButton.CurrentPartMessageId is not null)
                                    await _messageService.EditMessage(chatId, endButton.CurrentPartMessageId.Value, $"<b>{ currentPart.Text}</b>");
                            }

                            await _messageService.SendStartMessage(chatId);

                            break;
                        }
                    case NextStoryPartButton nextPartButton:
                        {
                            var button = await _storiesRepository.GetButton(nextPartButton.ButtonId);

                            if (button is null) throw new Exception($"Не найдена кнопка [{nextPartButton.ButtonId}]");

                            var currentPart = await _storiesRepository.GetStoryPartWithButtons(button.SourceStoryPartId);
                            if (currentPart is not null && nextPartButton.CurrentPartMessageId is not null)
                                await _messageService.EditMessage(chatId, nextPartButton.CurrentPartMessageId.Value, $"<b>{ currentPart.Text}</b>");

                            await _messageService.SendSimpleMessage(chatId, $"<i>{button.Text}</i>");

                            if (button.DestinationStoryPartId is null) throw new Exception("Ожидалось id следующей части истории");

                            var nextStoryPart = await _storiesRepository.GetStoryPartWithButtons(button.DestinationStoryPartId.Value);

                            if (nextStoryPart is null)
                            {
                                await _messageService.SendStartMessage(chatId);
                                return;
                            }

                            await _messageService.SendStoryPart(chatId, nextStoryPart);
                            break;
                        }
                    default:
                        throw new Exception($"Инвалидный колбэк charId:[{chatId}] [{callback}]");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Что-то пошло не так");

                await _messageService.SendStartMessage(chatId);
            }
        }
    }
}
