using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Factories;
using RentBot.Model;
using RentBot.Services.Interfaces;

namespace RentBot.Services.Implementation
{
    internal class BotService : IBotService
    {
        private readonly IClientFactory _clientFactory;
        private readonly ICommandService _commandService;
        private readonly ILogger _logger;

        public BotService(IClientFactory clientFactory, ICommandService commandService, ILogger logger)
        {
            _clientFactory = clientFactory;
            _commandService = commandService;
            _logger = logger;
        }

        public async Task ProcessAsync(TelegramRequest request)
        {
            try
            {
                var command = _commandService.GetCommandByMessage(request.Message);
                await command.Function(_clientFactory, request);
                await command.Fallback(_clientFactory, request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(BotService)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
            }
        }
    }
}
