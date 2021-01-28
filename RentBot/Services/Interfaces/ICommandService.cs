using RentBot.Commands.Interfaces;
using Telegram.Bot.Types;

namespace RentBot.Services.Interfaces
{
    public interface ICommandService
    {
        ICommand GetCommand(Update update);
    }
}