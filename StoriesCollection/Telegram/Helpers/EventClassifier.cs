using StoriesCollection.Telegram.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Linq;

namespace StoriesCollection.Telegram.Helpers
{
    public static class EventClassifier
    {
        public static EventType? CheckEvent(Update update)
        {
            if (update.Type == UpdateType.CallbackQuery) return EventType.Callback;
            if (update.Type == UpdateType.Message && update.Message.Entities?[0].Type != MessageEntityType.BotCommand) return EventType.Message;
            if (update.Type == UpdateType.Message && update.Message.Entities?[0].Type == MessageEntityType.BotCommand) return EventType.Command;
            return null;
        }
    }
}
