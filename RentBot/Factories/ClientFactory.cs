using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RentBot.Factories
{
    internal class ClientFactory : IClientFactory
    {
        private ITelegramBotClient _telegramBotClient;
        private BlobContainerClient _blobContainerClient;

        public BlobContainerClient GetBlobContainerClient()
        {
            if (_blobContainerClient == null)
            {
                var connectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STR", EnvironmentVariableTarget.Process);
                var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME", EnvironmentVariableTarget.Process);

                _blobContainerClient = new BlobContainerClient(connectionString, containerName);
            }
            return _blobContainerClient;
        }

        public async Task<ITelegramBotClient> GetTelegramBotClient()
        {
            if (_telegramBotClient == null)
            {
                var botSecret = Environment.GetEnvironmentVariable("BOT_SECRET", EnvironmentVariableTarget.Process);
                _telegramBotClient = new TelegramBotClient(botSecret);

                var startCommand = new BotCommand { Command = "/start", Description = "to begin conversation" };
                var commands = await _telegramBotClient.GetMyCommandsAsync();

                if (!commands.Any(existingCommand => existingCommand.Command == startCommand.Command.Remove(0,1))) // commands are saved without '/'symbol on server
                {
                    await _telegramBotClient.SetMyCommandsAsync(new List<BotCommand> { startCommand });
                }
            }
            return _telegramBotClient;
        }
    }
}
