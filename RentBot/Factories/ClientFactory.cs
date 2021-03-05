using System;
using Azure.Storage.Blobs;
using Google.Cloud.Translate.V3;
using Telegram.Bot;

namespace RentBot.Factories
{
    internal class ClientFactory : IClientFactory
    {
        private ITelegramBotClient _telegramBotClient;
        private BlobContainerClient _blobContainerClient;
        private TranslationServiceClient _translationClient;

        public BlobContainerClient GetBlobContainerClient()
        {
            if (_blobContainerClient == default)
            {
                var connectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STR", EnvironmentVariableTarget.Process);
                var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME", EnvironmentVariableTarget.Process);

                _blobContainerClient = new BlobContainerClient(connectionString, containerName);
            }
            return _blobContainerClient;
        }

        public ITelegramBotClient GetTelegramBotClient()
        {
            if (_telegramBotClient == default)
            {
                var botSecret = Environment.GetEnvironmentVariable("BOT_SECRET", EnvironmentVariableTarget.Process);

                _telegramBotClient = new TelegramBotClient(botSecret);
            }
            return _telegramBotClient;
        }

        public TranslationServiceClient GetTranslationClient()
        {
            if (_translationClient == default)
            {
                _translationClient = TranslationServiceClient.Create();
            }
            return _translationClient;
        }
    }
}
