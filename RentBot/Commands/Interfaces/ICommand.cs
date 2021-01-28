using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RentBot.Commands.Interfaces
{
    public interface ICommand
    {
        Task ExecuteAsync(Update update);
    }
}
