using System;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using RentBot.Services.Interfaces;
using System.Threading.Tasks;

namespace RentBot.Services.Implementation
{
    internal class BotService : IBotService
    {
        private readonly ICommandService _commandService;
        private readonly ILogger _logger;

        public BotService(ICommandService commandService, ILogger logger)
        {
            _commandService = commandService;
            _logger = logger;
        }

        public async Task ProcessAsync(Update update)
        {
            try
            {
                var command = _commandService.GetCommand(update);
                await command.ExecuteAsync(update);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(BotService)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
            }
        }
    }
}
