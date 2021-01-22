using RentBot.Services.Interfaces;
using Telegram.Bot.Types.Enums;

namespace RentBot.Factory
{
    public interface IHandlerFactory
    {
        IHandler GetHandlerOfType(UpdateType updateType);
    }
}
