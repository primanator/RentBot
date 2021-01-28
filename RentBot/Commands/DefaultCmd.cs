using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Commands
{
    class DefaultCmd : ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public DefaultCmd(ITelegramBotClient botClient, ILogger logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            await _botClient.SendChatActionAsync(update.Message.Chat.Id, ChatAction.Typing);

            await _botClient.SendTextMessageAsync(update.Message.Chat, update.Message.Text);
        }
    }
}
