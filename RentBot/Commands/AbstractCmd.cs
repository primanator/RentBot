using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RentBot.Commands
{
    abstract class AbstractCmd : ICommand
    {
        protected readonly ITelegramBotClient BotClient;
        protected readonly ILogger Logger;

        public string DetailedCommand { get; set; }

        public AbstractCmd(IClientFactory clientFactory, ILogger logger)
        {
            BotClient = clientFactory.GetTelegramBotClient();
            Logger = logger;
        }

        public abstract Task ExecuteAsync(Update update);
    }
}
