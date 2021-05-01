using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Telegram.Bot;

namespace RentBot.Factories
{
    public interface IClientFactory
    {
        Task<ITelegramBotClient> GetTelegramBotClient();
        BlobContainerClient GetBlobContainerClient();
    }
}
