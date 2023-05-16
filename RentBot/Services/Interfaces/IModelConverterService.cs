using Microsoft.AspNetCore.Http;
using RentBot.Model;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RentBot.Services.Interfaces;

public interface IModelConverterService
{
    TelegramRequest UpdateToTelegramRequest(Update update);
    Task<Update> HttpRequestToTelegramUpdateAsync(HttpRequest httpRequest);
}
