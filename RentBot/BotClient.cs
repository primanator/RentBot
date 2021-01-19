using Telegram.Bot;
using System;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;

namespace RentBot
{
    internal class BotClient
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public BotClient(ITelegramBotClient botClient, ILogger logger)
        {
            _botClient = botClient ?? throw new NullReferenceException("");
            _logger = logger ?? throw new NullReferenceException("");
        }

        public async void HandleUpdate(Update message)
        {
            _logger.LogInformation("Message Update Type: " + message.Type);

            if (message.Type == UpdateType.Message)
            {
                _logger.LogInformation($"Received a text message in chat {message.Message.Chat.Id}.");
                await _botClient.SendTextMessageAsync(message.Message.Chat, message.Message.Text);
            }
        }
    }
}
