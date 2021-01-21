using RentBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace RentBot.Factory
{
    public interface IUpdateHandlerFactory
    {
        IUpdateHandler GetHandlerOfType(UpdateType updateType);
    }
}
