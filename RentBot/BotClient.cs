using Telegram.Bot;
using System;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;

namespace RentBot
{
    internal class BotClient
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public BotClient(ITelegramBotClient botClient, ILogger logger)
        {
            _botClient = botClient ?? throw new NullReferenceException("");
            _logger = logger ?? throw new NullReferenceException("");
        }

        public async void HandleUpdate(Update message)
        {
            _logger.LogInformation("Message Update Type: " + message.Type);

            if (message.Type == UpdateType.Message)
            {
                _logger.LogInformation($"Received a text message in chat {message.Message.Chat.Id}.");

                if (message.Message.Text.Equals("/start", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _botClient.SendTextMessageAsync(message.Message.Chat.Id, GetWelcomingText(message.Message.From),
                        replyMarkup: new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
                        {
                            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("How do I get to your place?", "Path") },
                            new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("Show me what's around!", "Places") }
                        }));
                }
                else
                {
                    await _botClient.SendTextMessageAsync(message.Message.Chat, message.Message.Text);
                }
            }
        }

        private string GetWelcomingText(User user)
        {
            var welcomeText = $"Hi, {user.FirstName}";
            welcomeText += string.IsNullOrEmpty(user.LastName) ? string.Empty : " " + user.LastName;
            return welcomeText + "!";
        }
    }
}
