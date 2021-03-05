using Azure.Storage.Blobs;
using Google.Cloud.Translate.V3;
using Telegram.Bot;

namespace RentBot.Factories
{
    public interface IClientFactory
    {
        ITelegramBotClient GetTelegramBotClient();
        BlobContainerClient GetBlobContainerClient();
        TranslationServiceClient GetTranslationClient();
    }
}
