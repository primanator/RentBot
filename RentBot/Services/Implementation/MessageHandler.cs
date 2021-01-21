using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RentBot.Services.Implementation
{
    internal class MessageHandler : IUpdateHandler
    {
        private readonly ILogger _logger;

        public MessageHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task RespondAsync(ITelegramBotClient botClient, Update update)
        {
            if (update.Message.Text.Equals("/start", StringComparison.InvariantCultureIgnoreCase))
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, GetWelcomingText(update.Message.From),
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                            new [] { InlineKeyboardButton.WithCallbackData("How do I get to your place?", "Path") },
                            new [] { InlineKeyboardButton.WithCallbackData("Show me what's around!", "Places") }
                    }));
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, update.Message.Text);
            }
        }

        public async Task SendChatActionAsync(ITelegramBotClient botClient, Update update)
        {
            await botClient.SendChatActionAsync(update.Message.Chat.Id, ChatAction.Typing);
        }

        private string GetWelcomingText(User user)
        {
            var welcomeText = $"Hi, {user.FirstName}";
            welcomeText += string.IsNullOrEmpty(user.LastName) ? string.Empty : $" {user.LastName}";
            return welcomeText + "!";
        }
    }
}
