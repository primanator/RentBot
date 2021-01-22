﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RentBot.Services.Implementation
{
    internal class MessageHandler : IHandler
    {
        private readonly ICommandService _commandService;
        private readonly ILogger _logger;

        public MessageHandler(ICommandService commandService, ILogger logger)
        {
            _commandService = commandService;
            _logger = logger;
        }

        public async Task RespondAsync(ITelegramBotClient botClient, Update update)
        {
            if (_commandService.TryGetCommandForMessage(update.Message.Text, out var command))
            {
                await command.Execute(update);
                return;
            }
            await _commandService.GetDefaultCommand().Execute(update);
        }
    }
}