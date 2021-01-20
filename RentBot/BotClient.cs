using Telegram.Bot;
using System;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;

namespace RentBot
{
    internal class BotClient
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public BotClient(ITelegramBotClient botClient, ILogger logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public void Process(Update update)
        {
            try
            {
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
                            _logger.LogWarning("BotClient cant process update of unknown type");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BotClient process error: {ex.Message}\n StackTrace: {ex.StackTrace}");
            }
        }

        private async void HandeCallbackQuery(Update update)
        {
            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);

            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"Received {update.CallbackQuery.Data}");
        }

        private async void HandeInlineQuery(Update update)
        {
            await _botClient.AnswerInlineQueryAsync(update.InlineQuery.Id, new[] {
                new InlineQueryResultArticle(
                    // id's should be unique for each type of response
                    id: "1",
                    // Title of the option
                    title: "sample title",
                    // This is what is returned
                    new InputTextMessageContent("text that is returned") { ParseMode = ParseMode.Default })
                {
                    // This is just the description shown for the option
                    Description = "You could also put your output text for a preview here too."
                } });
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Received {update.InlineQuery.Query}");
        }

        private async void HandleMessage(Update update)
        {
            await _botClient.SendChatActionAsync(update.Message.Chat.Id, ChatAction.Typing);

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
