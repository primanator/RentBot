using System;
using System.Collections.Generic;
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

        public ITelegramBotClient GetTelegramBotClient()
        {
            if (_telegramBotClient == null)
            {
                var botSecret = Environment.GetEnvironmentVariable("BOT_SECRET", EnvironmentVariableTarget.Process);
                _telegramBotClient = new TelegramBotClient(botSecret);
                _telegramBotClient.SetMyCommandsAsync(new List<BotCommand> { new BotCommand { Command = "/start", Description = "to begin conversation" } });
            }
            return _telegramBotClient;
        }
    }
}
