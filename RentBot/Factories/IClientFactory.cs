using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Telegram.Bot;

namespace RentBot.Factories
{
    public interface IClientFactory
    {
        ITelegramBotClient GetTelegramBotClient();
        Task<CloudBlobContainer> GetCloudBlobContainerClient();
    }
}
