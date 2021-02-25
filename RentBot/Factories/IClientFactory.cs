using Azure.Storage.Blobs;
using Telegram.Bot;

namespace RentBot.Factories
{
    public interface IClientFactory
    {
        ITelegramBotClient GetTelegramBotClient();
        BlobContainerClient GetBlobContainerClient();
    }
}
