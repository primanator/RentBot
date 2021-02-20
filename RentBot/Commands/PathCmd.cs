using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Commands
{
    internal class PathCmd: ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CloudBlobContainer _blobContainer;
        private readonly ILogger _logger;

        public PathCmd(IClientFactory clientFactory, ILogger logger)
        {
            _botClient = clientFactory.GetTelegramBotClient();
            _blobContainer = clientFactory.GetCloudBlobContainerClient().GetAwaiter().GetResult();
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);

            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"path command: {update.CallbackQuery.Data}");

            var blobRef = _blobContainer.GetBlockBlobReference("bus_schedule.jpg");

            await _botClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, blobRef.Uri.AbsoluteUri, "Bus schedule");
        }
    }
}
