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
                var buttonData = JsonSerializer.Deserialize<ButtonData>(callback ?? "") ?? throw new Exception("Инвалидное поведение");

                if (buttonData.StartMessageId is not null) await _telegramGateway.DeleteMessage(chatId, (int)buttonData.StartMessageId);
                buttonData.StartMessageId = null;

                if (buttonData.ButtonId is not null && buttonData.CurrentPartMessageId is not null)
                {
                    var button = await _storiesRepository.GetButton((int)buttonData.ButtonId);
                    if (button is null)
                    {
                        await _messageService.SendStartMessage(chatId);
                    }
                    else
                    {
                        var currentPart = await _storiesRepository.GetStoryPart(buttonData.StoryId, button.SourceStoryPartId ?? "");
                        if (currentPart is not null) await _messageService.EditMessage(chatId, (int)buttonData.CurrentPartMessageId, $"<b>{ currentPart.Text}</b>");
                    }                            
                }

                switch (buttonData.Type)
                {
                    case ButtonType.StartStory:
                        {
                            var storyInfo = await _storiesRepository.GetStoryInfo(buttonData.StoryId);

                            await _messageService.SendSimpleMessage(chatId, $"------<b><i>{storyInfo?.Name}</i></b>------");

                            if (storyInfo?.FirstStoryPartId is null)
                            {
                                await _messageService.SendStartMessage(chatId);
                                return;
                            }

                            var firstStoryPart = await _storiesRepository.GetStoryPart(storyInfo.Id, storyInfo.FirstStoryPartId ?? "");

                            if (firstStoryPart is null)
                            {
                                await _messageService.SendStartMessage(chatId);
                                return;
                            }

                            await _messageService.SendStoryPart(chatId, firstStoryPart as StoryPart);

                            break;
                        }
                    case ButtonType.NextPart:
                        {
                            if (buttonData.ButtonId is null) throw new Exception("Инвалидное поведение");

                            var button = await _storiesRepository.GetButton((int)buttonData.ButtonId);
                            if (button is null)
                            {
                                await _messageService.SendStartMessage(chatId);
                                return;
                            }

                            await _messageService.SendSimpleMessage(chatId, $"<i>{button.Text}</i>");

                            var nextStoryPart = await _storiesRepository.GetStoryPart(buttonData.StoryId, button.DestioationStoryPartId ?? "");

                            if (nextStoryPart is null)
                            {
                                await _messageService.SendStartMessage(chatId);
                                return;
                            }

                            await _messageService.SendStoryPart(chatId, nextStoryPart as StoryPart);
                            break;
                        }
                    case ButtonType.EndStory:
                        {
                            if (buttonData.ButtonId is not null)
                            {
                                var button = await _storiesRepository.GetButton((int)buttonData.ButtonId);

                                if (button is not null)
                                {
                                    await _messageService.SendSimpleMessage(chatId, $"<i>{button.Text}</i>");
                                }
                            }

                            await _messageService.SendStartMessage(chatId);
                            break;
                        }
                    default:
                        {
                            throw new Exception("Неверный тип кнопки");
                        }
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
