using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RentBot.Commands
{
    abstract class AbstractCommand : ICommand
    {
        protected readonly ITelegramBotClient BotClient;
        protected readonly ILogger Logger;

        public List<string> AvailableMessages { get; protected set; }
        public string SelectedMessage { get; set; }

        public AbstractCommand(IClientFactory clientFactory, ILogger logger)
        {
            BotClient = clientFactory.GetTelegramBotClient();
            Logger = logger;
        }

        public abstract Task ExecuteAsync(Update update);
    }
}
