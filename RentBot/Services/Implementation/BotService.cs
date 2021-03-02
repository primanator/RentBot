using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands;
using RentBot.Factories;
using RentBot.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
                new PlacesCommand(clientFactory, logger)
            };
        }

        public async Task ProcessAsync(Update update)
        {
            try
            {
                var message = GetMessage(update);
                var command = GetCommandByMessage(message);
                command.SelectedMessage = message;

                await command.ExecuteAsync(update);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(BotService)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
            }
        }

        private string GetMessage(Update update)
        {
            var message = string.Empty;
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        message = update.Message.Text;
                        break;
                    }
                case UpdateType.CallbackQuery:
                    {
                        message = update.CallbackQuery.Data;
                        break;
                    }
                default:
                    {
                        _logger.LogError($"{nameof(BotService)} can't get message for update type {update.Type}!");
                        break;
                    }
            }
            return message;
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
