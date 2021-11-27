using Microsoft.Extensions.Logging;
using StoriesCollection.Db.Models;
using StoriesCollection.Db.Repository;
using StoriesCollection.Helpers;
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
                if (message is null || !message.StartsWith("admin")) return;

                var parts = message.Split(";").Skip(1);

                var model = parts.ElementAt(0);
                var command = parts.ElementAt(1);

                switch (model)
                {
                    case "story":
                        {
                            switch(command)
                            {
                                case "add":
                                    {
                                        if (!await CheckPartsCount(parts, 3, chatId)) return;

                                        var newStoryName = parts.ElementAt(2);

                                        if (await _storiesRepository.IsStoryExist(newStoryName))
                                            await _messageService.SendSimpleMessage(chatId, "История с таким именем уже сущеструет");

                                        await _storiesRepository.AddStory(newStoryName);
                                        await _storiesRepository.SaveChanges();

                                        var story = await _storiesRepository.GetStory(newStoryName);

                                        if (story is not null) await _messageService.SendSimpleMessage(chatId, $"{story.Id}");

                                        break;
                                    }
                            }
                            break;
                        }
                    case "story part":
                        {
                            switch (command)
                            {
                                case "add":
                                    {
                                        if (!await CheckPartsCount(parts, 4, chatId)) return;

                                        var sTargetStoryId = parts.ElementAt(2);
                                        var newStoryPartText = parts.ElementAt(3);

                                        if (!int.TryParse(sTargetStoryId, out int targetStoryId))
                                        {
                                            await _messageService.SendSimpleMessage(chatId, $"Невалидный id [{targetStoryId}]");
                                            return;
                                        }

                                        var story = await _storiesRepository.GetStoryInfo(targetStoryId);
                                        if (story is null)
                                        {
                                            await _messageService.SendSimpleMessage(chatId, "Истории с таким именем не сущеструет");
                                            return;
                                        }

                                        await _storiesRepository.AddStoryPart(story, newStoryPartText);
                                        await _storiesRepository.SaveChanges();

                                        var storyPart = await _storiesRepository.GetStoryPart(targetStoryId, newStoryPartText);

                                        if (storyPart is not null)
                                        {
                                            var updatedStory = await _storiesRepository.GetStoryInfo(targetStoryId);
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
                            }
                            break;
                        }
                    case "button":
                        {
                            switch (command)
                            {
                                case "add":
                                    {
                                        if (!await CheckPartsCount(parts, 5, chatId)) return;

                                        var sSourceStoryPartId = parts.ElementAt(2);
                                        var sDestinationStoryPartId = parts.ElementAt(3);
                                        var buttonText = parts.ElementAt(4);

                                        if(!int.TryParse(sSourceStoryPartId, out int sourceStoryPartId))
                                        {
                                            await _messageService.SendSimpleMessage(chatId, $"Невалидный id [{sSourceStoryPartId}]");
                                            return;
                                        }

                                        var sourceStoryPart = await _storiesRepository.GetStoryPart(sourceStoryPartId);

                                        if (sourceStoryPart is null)
                                        {
                                            await _messageService.SendSimpleMessage(chatId, $"Часть опроса с id [{sourceStoryPartId}] не найдена");
                                            return;
                                        }

                                        StoryPart? destinationStoryPart = null;
                                        if (!string.IsNullOrEmpty(sDestinationStoryPartId))
                                        {
                                            if (!int.TryParse(sDestinationStoryPartId, out int destinationStoryPartId))
                                            {
                                                await _messageService.SendSimpleMessage(chatId, $"Невалидный id [{sDestinationStoryPartId}]");
                                                return;
                                            }

                                            destinationStoryPart = await _storiesRepository.GetStoryPart(destinationStoryPartId);
                                        }

                                        await _storiesRepository.AddButton(sourceStoryPart, destinationStoryPart, buttonText);
                                        await _storiesRepository.SaveChanges();

                                        await _messageService.SendSimpleMessage(chatId ,$"Кнопка добавлена успешно");

                                        break;
                                    }
                            }
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

        private async Task<bool> CheckPartsCount(IEnumerable<string> parts, int expectedCount, long chatId)
        {
            if (parts.Count() >= expectedCount) return true;
            
            await _messageService.SendSimpleMessage(chatId, $"Ожидалось {expectedCount} частей команды, пришло [{parts.Count()}]");
            return true;
        }
    }
}
