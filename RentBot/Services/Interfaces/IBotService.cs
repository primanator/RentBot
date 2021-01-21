using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RentBot.Services.Interfaces
{
    public interface IBotService
    {
        Task ProcessAsync(Update update);
    }
}
