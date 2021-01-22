using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RentBot.Commands
{
    public interface ICommand
    {
        Task Execute(Update update);
    }
}
