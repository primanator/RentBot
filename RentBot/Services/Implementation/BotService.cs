using Telegram.Bot;
using System;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using RentBot.Services.Interfaces;
using RentBot.Factory;
using System.Threading.Tasks;

namespace RentBot.Services.Implementation
{
    internal class BotService : IBotService
    {
        private readonly IHandlerFactory _handlerFactory;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public BotService(IHandlerFactory handlerFactory, ITelegramBotClient botClient, ILogger logger)
        {
            _handlerFactory = handlerFactory;
            _botClient = botClient;
            _logger = logger;
        }

        public async Task ProcessAsync(Update update)
        {
            try
            {
                var handler = _handlerFactory.GetHandlerOfType(update.Type);
                await handler.RespondAsync(_botClient, update);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(BotService)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
            }
        }
    }
}
