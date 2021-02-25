using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Telegram.Bot;

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
            }
            return _telegramBotClient;
        }
    }
}
