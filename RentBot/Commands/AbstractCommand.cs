using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Constants;
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

        public List<string> AvailableMessages { get; protected set; }

        public AbstractCommand(IClientFactory clientFactory, ILogger logger)
        {
            BotClient = clientFactory.GetTelegramBotClient();
            Logger = logger;
        }

        public abstract Task ExecuteAsync(TelegramRequest request);

        protected async Task FallbackAsync(long chatId, string text, InlineKeyboardMarkup keyboardMarkup)
        {
            await BotClient.SendTextMessageAsync(chatId, text, replyMarkup: keyboardMarkup);
        }
    }
}
