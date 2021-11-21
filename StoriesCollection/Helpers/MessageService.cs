using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StoriesCollection.Db.Models;
using StoriesCollection.Db.Repository;
using StoriesCollection.Models;
using StoriesCollection.Telegram;

namespace StoriesCollection.Helpers
{
    public class MessageService
    {
        private readonly StoriesRepository _storiesRepository;
        private readonly TelegramGateway _telegramGateway;
        private readonly ILogger<MessageService> _logger;
        public MessageService(ILogger<MessageService> logger, StoriesRepository storiesRepository, TelegramGateway telegramGateway)
        {
            _logger = logger;
            _storiesRepository = storiesRepository;
            _telegramGateway = telegramGateway;
        }

        public async Task EditMessage(long chatId, int messageId, string text)
        {
            await _telegramGateway.EditMessage(chatId, messageId, null, text);
        }

        public async Task<int> SendStartMessage(long chatId)
        {
            var allStories = await _storiesRepository.GetAllStory();
            var message = await _telegramGateway.SendMessage(chatId, allStories.OrderBy(x => x.Name).ToDictionary(x => x.Name, x => new ButtonData
            {
                StoryId = x.Id,
                Type = ButtonType.StartStory,
            }));
            await _telegramGateway.EditMessage(chatId, message.MessageId, allStories.OrderBy(x => x.Name).ToDictionary(x => x.Name, x => new ButtonData
            {
                StoryId = x.Id,
                Type = ButtonType.StartStory,
                StartMessageId = message.MessageId
            }));

            return message.MessageId;
        }

        public async Task SendStoryPart(long chatId, StoryPart storyPart)
        {
            var message = await _telegramGateway.SendMessage(chatId, storyPart.ButtonsNext.OrderBy(x => x.Text).ToDictionary(x => x.Text, x => new ButtonData
            {
                StoryId = storyPart.StoryId,
                Type = x.DestioationStoryPartId is null ? ButtonType.EndStory : ButtonType.NextPart,
                ButtonId = x.Id
            }), $"<b>{storyPart.Text}</b>");

            await _telegramGateway.EditMessage(chatId, message.MessageId, storyPart.ButtonsNext.OrderBy(x => x.Text).ToDictionary(x => x.Text, x => new ButtonData
            {
                StoryId = storyPart.StoryId,
                Type = x.DestioationStoryPartId is null ? ButtonType.EndStory : ButtonType.NextPart,
                ButtonId = x.Id,
                CurrentPartMessageId = message.MessageId,
            }), $"<b>{storyPart.Text}</b>");
        }

        public async Task SendSimpleMessage(long chatId, string text)
            => await _telegramGateway.SendMessage(chatId, null, text);
    }
}