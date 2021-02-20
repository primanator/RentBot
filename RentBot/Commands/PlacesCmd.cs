using System;
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
    internal class PlacesCmd : ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CloudBlobContainer _blobContainer;
        private readonly ILogger _logger;

        public PlacesCmd(IClientFactory clientFactory, ILogger logger)
        {
            _botClient = clientFactory.GetTelegramBotClient();
            _blobContainer = clientFactory.GetCloudBlobContainerClient().GetAwaiter().GetResult();
            _logger = logger;
        }

        [Obsolete]
        public async Task ExecuteAsync(Update update)
        {
            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"places command: {update.CallbackQuery.Data}");

            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
            await _botClient.SendMediaGroupAsync(update.CallbackQuery.Message.Chat.Id, new InputMediaBase[]
            {
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("home1.jpg").Uri.AbsoluteUri) { Caption = "Home" },
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("home2.jpg").Uri.AbsoluteUri),
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("home3.jpg").Uri.AbsoluteUri),
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("home4.jpg").Uri.AbsoluteUri)
            });

            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
            await _botClient.SendMediaGroupAsync(update.CallbackQuery.Message.Chat.Id, new InputMediaBase[]
            {
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("restaurant1.jpg").Uri.AbsoluteUri) { Caption = "Restaurant" },
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("restaurant2.jpg").Uri.AbsoluteUri),
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("restaurant3.jpg").Uri.AbsoluteUri)
            });

            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
            await _botClient.SendMediaGroupAsync(update.CallbackQuery.Message.Chat.Id, new InputMediaBase[]
            {
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("river1.jpg").Uri.AbsoluteUri) { Caption = "River" },
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("river2.jpg").Uri.AbsoluteUri),
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("river3.jpg").Uri.AbsoluteUri),
                new InputMediaPhoto(_blobContainer.GetBlockBlobReference("river4.jpg").Uri.AbsoluteUri)
            });
        }
    }
}
