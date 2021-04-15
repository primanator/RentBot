using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands;
using RentBot.Factories;
using RentBot.Model;
using RentBot.Services.Interfaces;

namespace RentBot.Services.Implementation
{
    internal class BotService : IBotService
    {
        private readonly ILogger _logger;
        private readonly List<AbstractCommand> _commandsList;
        private readonly AbstractCommand _defaultCommand;

        public BotService(IClientFactory clientFactory, ILogger logger)
        {
            _logger = logger;
            _defaultCommand = new StartCommand(clientFactory, logger);
            _commandsList = new List<AbstractCommand>
            {
                _defaultCommand,
                new PathCommand(clientFactory, logger),
                new PlacesCommand(clientFactory, logger),
                new MiscCommand(clientFactory, logger)
            };
        }

        public async Task ProcessAsync(TelegramRequest request)
        {
            try
            {
                var command = GetCommandByMessage(request.Message);
                await command.ExecuteAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(BotService)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
            }
        }

        private AbstractCommand GetCommandByMessage(string message)
        {
            var command = _commandsList.FirstOrDefault(cmd => cmd.AvailableMessages.Contains(message));
            if (command == default(AbstractCommand))
            {
                command = _defaultCommand;
            }
            return command;
        }
    }
}
