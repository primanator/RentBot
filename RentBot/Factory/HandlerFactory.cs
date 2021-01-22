using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RentBot.Services.Implementation;
using RentBot.Services.Interfaces;
using Telegram.Bot.Types.Enums;

namespace RentBot.Factory
{
    internal class HandlerFactory : IHandlerFactory
    {
        private readonly ILogger _logger;
        private readonly ICommandService _commandService;
        private Dictionary<UpdateType, IHandler> _handlersDict;

        public HandlerFactory(ICommandService commandService, ILogger logger)
        {
            _logger = logger;
            _commandService = commandService;
            _handlersDict = new Dictionary<UpdateType, IHandler>();
        }

        public IHandler GetHandlerOfType(UpdateType updateType)
        {
            if (_handlersDict.TryGetValue(updateType, out var existingHandler))
            {
                return existingHandler;
            }

            IHandler handler = default;
            switch (updateType)
            {
                case UpdateType.Message:
                    {
                        handler = new MessageHandler(_commandService, _logger);
                        break;
                    }
                case UpdateType.CallbackQuery:
                    {
                        handler = new CallbackQueryHandler(_commandService, _logger);
                        break;
                    }
                default:
                    {
                        _logger.LogError($"{nameof(HandlerFactory)} can't get handler of type {updateType}!");
                        break;
                    }
            }

            _handlersDict.Add(updateType, handler);
            return handler;
        }
    }
}
