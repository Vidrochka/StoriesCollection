using Microsoft.Extensions.Logging;
using StoriesCollection.Db.Models;
using StoriesCollection.Db.Repository;
using StoriesCollection.Helpers;
using StoriesCollection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoriesCollection.Handlers
{
    public class MessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;
        private readonly MessageService _messageService;
        private readonly StoriesRepository _storiesRepository;

        public MessageHandler(ILogger<MessageHandler> logger, MessageService messageService, StoriesRepository storiesRepository)
        {
            _logger = logger;
            _messageService = messageService;
            _storiesRepository = storiesRepository;
        }

        public async Task Handle(long chatId, string? message)
        {
            try
            {
                switch (MessageFactory.GetMessage(message))
                {
                    case AddStory addStory:
                        {
                            if (!addStory.IsValid)
                            {
                                await _messageService.SendSimpleMessage(chatId, $"Инвалидная команда");
                                break;
                            }


                            if (await _storiesRepository.IsStoryExist(addStory.StoryName!))
                                await _messageService.SendSimpleMessage(chatId, "История с таким именем уже сущеструет");

                            await _storiesRepository.AddStory(addStory.StoryName!);
                            await _storiesRepository.SaveChanges();

                            var story = await _storiesRepository.GetStory(addStory.StoryName!);

                            if (story is not null) await _messageService.SendSimpleMessage(chatId, $"{story.Id}");

                            break;
                        }
                    case AddStoryPart addStoryPart:
                        {
                            if (!addStoryPart.IsValid)
                            {
                                await _messageService.SendSimpleMessage(chatId, $"Инвалидная команда");
                                break;
                            }

                            var story = await _storiesRepository.GetStoryInfo(addStoryPart.StoryId!.Value);
                            if (story is null)
                            {
                                await _messageService.SendSimpleMessage(chatId, "Истории с таким именем не сущеструет");
                                return;
                            }

                            await _storiesRepository.AddStoryPart(story, addStoryPart.StoryPartText!);
                            await _storiesRepository.SaveChanges();

                            var storyPart = await _storiesRepository.GetStoryPart(addStoryPart.StoryId!.Value, addStoryPart.StoryPartText!);

                            if (storyPart is not null)
                            {
                                var updatedStory = await _storiesRepository.GetStoryInfo(addStoryPart.StoryId!.Value);
                                if (updatedStory is not null && updatedStory.FirstStoryPartId is null)
                                {
                                    updatedStory.FirstStoryPartId = storyPart.Id;
                                    _storiesRepository.UpdateStory(updatedStory);
                                    await _storiesRepository.SaveChanges();
                                }

                                await _messageService.SendSimpleMessage(chatId, $"{storyPart.Id}");
                            }

                            break;
                        }
                    case AddButton addButton:
                        {
                            if (!addButton.IsValid)
                            {
                                await _messageService.SendSimpleMessage(chatId, $"Инвалидная команда");
                                break;
                            }

                            var sourceStoryPart = await _storiesRepository.GetStoryPart(addButton.StoryPartFromId!.Value);

                            if (sourceStoryPart is null)
                            {
                                await _messageService.SendSimpleMessage(chatId, $"Часть опроса с id [{addButton.StoryPartFromId!.Value}] не найдена");
                                return;
                            }

                            StoryPart? destinationStoryPart = null;

                            if (addButton.StoryPartToId is not null)
                                destinationStoryPart = await _storiesRepository.GetStoryPart(addButton.StoryPartToId.Value);

                            await _storiesRepository.AddButton(sourceStoryPart, destinationStoryPart, addButton.ButtonText!);
                            await _storiesRepository.SaveChanges();

                            await _messageService.SendSimpleMessage(chatId, $"Кнопка добавлена успешно");


                            break;
                        }
                    default:
                        {
                            await _messageService.SendStartMessage(chatId);
                            break;
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
