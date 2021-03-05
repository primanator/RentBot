using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using RentBot.Model;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace RentBot.Commands
{
    abstract class AbstractCommand : ICommand
    {
        protected readonly ITelegramBotClient BotClient;
        protected readonly ILogger Logger;
        protected readonly TranslationServiceClient TranslationClient;

        public List<string> AvailableMessages { get; protected set; }

        public AbstractCommand(IClientFactory clientFactory, ILogger logger)
        {
            BotClient = clientFactory.GetTelegramBotClient();
            Logger = logger;
            TranslationClient = clientFactory.GetTranslationClient();
        }

        public abstract Task ExecuteAsync(TelegramRequest request);

        protected async Task FallbackAsync(long chatId, string text, InlineKeyboardMarkup keyboardMarkup)
        {
            await BotClient.SendTextMessageAsync(chatId, text, replyMarkup: keyboardMarkup);
        }

        protected async Task TryAnswerCallbackQueryAsync(TelegramRequest request)
        {
            if (!string.IsNullOrEmpty(request.CallbackQueryId))
            {
                await BotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            }
        }

        protected async Task<string> TranslateAsync(string text, string targetLanguageCode)
        {
            var response = await TranslationClient.TranslateTextAsync(new TranslateTextRequest
            {
                Contents = { text },
                TargetLanguageCode = targetLanguageCode,
                Parent = new ProjectName("rentbot-306712").ToString()
            });

            Logger.LogInformation($"Detected language: {response.Translations[0].DetectedLanguageCode}");
            Logger.LogInformation($"Translated text: {response.Translations[0].TranslatedText}");

            // response.Translations will have one entry, because request.Contents has one entry.
            return response.Translations[0].TranslatedText;
        }
    }
}
