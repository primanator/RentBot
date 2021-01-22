using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RentBot.Commands
{
    public abstract class AbstractCommand : ICommand
    {
        protected readonly ITelegramBotClient _botClient;

        public abstract string Name { get; }

        public AbstractCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public abstract Task Execute(Update update);
    }
}