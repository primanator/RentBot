using Microsoft.AspNetCore.Http;
using RentBot.Models;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RentBot.Services.Interfaces;

public interface IModelConverterService
{
    TelegramRequest UpdateToTelegramRequest(Update update);
    Task<Update> HttpRequestToTelegramUpdateAsync(HttpRequest httpRequest);
}
