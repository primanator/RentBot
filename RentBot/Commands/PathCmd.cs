using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Commands
{
    internal class PathCmd: ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public PathCmd(ITelegramBotClient botClient, ILogger logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);

            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"path command: {update.CallbackQuery.Data}");
        }
    }
}
