using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Services.Implementation
{
    internal class CallbackQueryHandler : IUpdateHandler
    {
        private readonly ILogger _logger;

        public CallbackQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task RespondAsync(ITelegramBotClient botClient, Update update)
        {
            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"Received {update.CallbackQuery.Data}");
        }

        public async Task SendChatActionAsync(ITelegramBotClient botClient, Update update)
        {
            await botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
        }
    }
}
