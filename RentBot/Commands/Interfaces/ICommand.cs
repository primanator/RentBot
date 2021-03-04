using System.Threading.Tasks;
using RentBot.Model;

namespace RentBot.Commands.Interfaces
{
    public interface ICommand
    {
        Task ExecuteAsync(TelegramRequest request);
    }
}
