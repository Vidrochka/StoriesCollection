using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StoriesCollection.Db.Models;
using StoriesCollection.Db.Repository;
using StoriesCollection.Models;
using StoriesCollection.Telegram;
using Telegram.Bot.Types.Enums;

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
            var message = await _telegramGateway.SendMessage(chatId, allStories.OrderBy(x => x.Name).ToDictionary(x => x.Name, x => new StartStoryButton
            {
                Type = ButtonType.StartStory,
                StoryId = x.Id,
            }.ToRequest()));
            await _telegramGateway.EditMessage(chatId, message.MessageId, allStories.OrderBy(x => x.Name).ToDictionary(x => x.Name, x => new StartStoryButton
            {
                Type = ButtonType.StartStory,
                StoryId = x.Id,
                StartMessageId = message.MessageId,
            }.ToRequest()));

            return message.MessageId;
        }

        public async Task SendStoryPart(long chatId, StoryPart storyPart)
        {
            var message = await _telegramGateway.SendMessage(chatId, storyPart.ButtonsNext.OrderBy(x => x.Text).ToDictionary(x => x.Text, x => x.DestinationStoryPartId is null ?
            new EndStoryButton
            {
                Type = x.DestinationStoryPartId is null ? ButtonType.EndStory : ButtonType.NextPart,
                ButtonId = x.Id,
            }.ToRequest() :
            new NextStoryPartButton
            {
                Type = x.DestinationStoryPartId is null ? ButtonType.EndStory : ButtonType.NextPart,
                ButtonId = x.Id,
            }.ToRequest()), $"<b>{storyPart.Text}</b>");

            await _telegramGateway.EditMessage(chatId, message.MessageId, storyPart.ButtonsNext.OrderBy(x => x.Text).ToDictionary(x => x.Text, x => x.DestinationStoryPartId is null ?
            new EndStoryButton
            {
                Type = x.DestinationStoryPartId is null ? ButtonType.EndStory : ButtonType.NextPart,
                ButtonId = x.Id,
                CurrentPartMessageId = message.MessageId,
            }.ToRequest() :
            new NextStoryPartButton
            {
                Type = x.DestinationStoryPartId is null ? ButtonType.EndStory : ButtonType.NextPart,
                ButtonId = x.Id,
                CurrentPartMessageId = message.MessageId,
            }.ToRequest()), $"<b>{storyPart.Text}</b>");
        }

        public async Task SendSimpleMessage(long chatId, string text, ParseMode parseMode = ParseMode.Html)
            => await _telegramGateway.SendMessage(chatId, null, text, parseMode);
    }
}