using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RentBot.Services.Implementation;
using RentBot.Services.Interfaces;
using Telegram.Bot.Types.Enums;

namespace RentBot.Factory
{
    internal class UpdateHandlerFactory : IUpdateHandlerFactory
    {
        private readonly ILogger _logger;
        private Dictionary<UpdateType, IUpdateHandler> _handlersDict;

        public UpdateHandlerFactory(ILogger logger)
        {
            _logger = logger;
            _handlersDict = new Dictionary<UpdateType, IUpdateHandler>();
        }

        public IUpdateHandler GetHandlerOfType(UpdateType updateType)
        {
            if (_handlersDict.TryGetValue(updateType, out var existingHandler))
            {
                return existingHandler;
            }

            IUpdateHandler handler = default;
            switch (updateType)
            {
                case UpdateType.Message:
                    {
                        handler = new MessageHandler(_logger);
                        break;
                    }
                case UpdateType.CallbackQuery:
                    {
                        handler = new CallbackQueryHandler(_logger);
                        break;
                    }
                default:
                    {
                        _logger.LogError($"{nameof(UpdateHandlerFactory)} can't get handler of type {updateType}!");
                        break;
                    }
            }

            _handlersDict.Add(updateType, handler);
            return handler;
        }
    }
}
