﻿using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RentBot.Commands;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using RentBot.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Services.Implementation
{
    internal class CommandService : ICommandService
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, ICommand> _commandDict;

        public CommandService(IClientFactory clientFactory, ILogger logger)
        {
            _logger = logger;
            _commandDict = new Dictionary<string, ICommand>()
            {
                { ListOfCommands.Start, new StartCmd(clientFactory, logger) },
                { ListOfCommands.Path, new PathCmd(clientFactory, logger) },
                { ListOfCommands.Places, new PlacesCmd(clientFactory, logger) },
                { ListOfCommands.Default, new DefaultCmd(clientFactory, logger) }
            };
        }

        public ICommand GetCommand(Update update)
        {
            var updateMessage = GetMessage(update);

            if (_commandDict.TryGetValue(updateMessage, out var existingCommand))
            {
                return existingCommand;
            }
            return _commandDict[ListOfCommands.Default];
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
                        _logger.LogError($"{nameof(CommandService)} can't get message for update type {update.Type}!");
                        break;
                    }
            }
            return message;
        }
    }
}
