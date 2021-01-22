using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RentBot.Services.Interfaces
{
    public interface IHandler
    {
        Task SendRespondActionAsync(ITelegramBotClient botClient, Update update);
        Task RespondAsync(ITelegramBotClient botClient, Update update);
    }
}
