using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Telegram.Bot;

namespace RentBot.Factories
{
    internal class ClientFactory : IClientFactory
    {
        private ITelegramBotClient _telegramBotClient;
        private CloudBlobContainer _blobContainerClient;

        public async Task<CloudBlobContainer> GetCloudBlobContainerClient()
        {
            if (_blobContainerClient == null)
            {
                var connectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STR", EnvironmentVariableTarget.Process);
                var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME", EnvironmentVariableTarget.Process);
                
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                _blobContainerClient = blobClient.GetContainerReference(containerName);
                await _blobContainerClient.CreateIfNotExistsAsync();
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
