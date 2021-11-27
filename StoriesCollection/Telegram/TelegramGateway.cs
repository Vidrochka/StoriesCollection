using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoriesCollection.Models;
using StoriesCollection.Telegram.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace StoriesCollection.Telegram
{
    public class TelegramGateway
    {
        private readonly TelegramBotClient _client;
        private readonly ILogger<TelegramGateway> _logger;

        public TelegramGateway(IOptions<BotConfig> config, ILogger<TelegramGateway> logger)
        {
            _client = new TelegramBotClient(config.Value.BotToken);
            _logger = logger;
        }

        public async Task<Message> SendMessage(long chatId, Dictionary<string, string>? buttons = null, string text = "Список доступных историй", ParseMode parseMode = ParseMode.Html)
        {
            List<List<InlineKeyboardButton>>? buttonCollection = buttons?.Select(x => new List<InlineKeyboardButton> { new InlineKeyboardButton(x.Key) { CallbackData = x.Value }})?.ToList();

            var replyMarkup = buttonCollection is null ? null : new InlineKeyboardMarkup(
                buttonCollection
            );
            return await _client.SendTextMessageAsync(chatId, text, parseMode, replyMarkup: replyMarkup);
        }

        public async Task EditMessage(long chatId, int messageId, Dictionary<string, string>? buttons = null, string text = "Список доступных историй")
        {
            try
            {
                var buttonCollection = buttons?.Select(x => new List<InlineKeyboardButton> { new InlineKeyboardButton(x.Key) { CallbackData = x.Value }});
                await _client.EditMessageTextAsync(chatId, messageId, text, ParseMode.Html, replyMarkup: buttonCollection is null ? null : new InlineKeyboardMarkup(
                    buttonCollection
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка отправки сообщения без кнопок");
            }
        }

        public async Task DeleteMessage(long chatId, int messageId)
        {
            try
            {
                await _client.DeleteMessageAsync(chatId, messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка отправки сообщения без кнопок");
            }
        }
    }
}
