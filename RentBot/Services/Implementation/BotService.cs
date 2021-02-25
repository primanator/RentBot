using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using RentBot.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Services.Implementation
{
    internal class BotService : IBotService
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, AbstractCmd> _commandDict;

        public BotService(IClientFactory clientFactory, ILogger logger)
        {
            _logger = logger;
            var startCommand = new StartCmd(clientFactory, logger);
            var pathCommand = new PathCmd(clientFactory, logger);
            var placesCommand = new PlacesCmd(clientFactory, logger);

            _commandDict = new Dictionary<string, AbstractCmd>()
            {
                { ListOfCommands.Start, startCommand },
                { ListOfCommands.Default, startCommand },
                { ListOfCommands.Path, pathCommand },
                { ListOfCommands.HomeGeo, pathCommand },
                { ListOfCommands.BusSchedule, pathCommand },
                { ListOfCommands.Places, placesCommand },
                { ListOfCommands.Home, placesCommand },
                { ListOfCommands.Restaurant, placesCommand },
                { ListOfCommands.River, placesCommand }
            };
        }

        public async Task ProcessAsync(Update update)
        {
            try
            {
                var commandToExecute = default(AbstractCmd);
                var updateMessage = GetMessage(update);

                if (_commandDict.TryGetValue(updateMessage, out var existingCommand))
                {
                    commandToExecute = existingCommand;
                }
                else
                {
                    commandToExecute = _commandDict[ListOfCommands.Default];
                }

                commandToExecute.DetailedCommand = updateMessage;
                await commandToExecute.ExecuteAsync(update);
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
    }
}
