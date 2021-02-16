using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Commands
{
    internal class PlacesCmd: ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public PlacesCmd(IClientFactory clientFactory, ILogger logger)
        {
            _botClient = clientFactory.GetTelegramBotClient();
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);

            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"places command: {update.CallbackQuery.Data}");
        }
    }
}
