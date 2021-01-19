using Telegram.Bot;
using System;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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

        public async void Process(Update update)
        {
            await _botClient.SendChatActionAsync(update.Message.Chat.Id, ChatAction.Typing);

            _logger.LogInformation("Update Type: " + update.Type);
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        HandleMessage(update);
                        break;
                    }
                case UpdateType.CallbackQuery:
                    {
                        HandeCallbackQuery(update);
                        break;
                    }
                case UpdateType.InlineQuery:
                    {
                        HandeInlineQuery(update);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private async void HandeCallbackQuery(Update update)
        {
            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"Received {update.CallbackQuery.Data}");
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Received {update.CallbackQuery.Data}");
        }

        private async void HandeInlineQuery(Update update)
        {
            await _botClient.AnswerCallbackQueryAsync(update.InlineQuery.Id, $"Received {update.InlineQuery.Query}");
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Received {update.InlineQuery.Query}");
        }

        private async void HandleMessage(Update update)
        {
            if (update.Message.Text.Equals("/start", StringComparison.InvariantCultureIgnoreCase))
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, GetWelcomingText(update.Message.From),
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                            new [] { InlineKeyboardButton.WithCallbackData("How do I get to your place?", "Path") },
                            new [] { InlineKeyboardButton.WithCallbackData("Show me what's around!", "Places") }
                    }));
            }
            else
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat, update.Message.Text);
            }
        }

        private string GetWelcomingText(User user)
        {
            var welcomeText = $"Hi, {user.FirstName}";
            welcomeText += string.IsNullOrEmpty(user.LastName) ? string.Empty : $" {user.LastName}";
            return welcomeText + "!";
        }
    }
}
