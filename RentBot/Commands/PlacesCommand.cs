using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Constants;
using RentBot.Factories;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Emojis = RentBot.Constants.Emojis;

namespace RentBot.Commands
{
    internal class PlacesCommand : AbstractCommand, ICommand
    {
        private readonly BlobContainerClient _blobContainerClient;

        public PlacesCommand(IClientFactory clientFactory, ILogger logger) : base(clientFactory, logger)
        {
            _blobContainerClient = clientFactory.GetBlobContainerClient();
            AvailableMessages = new List<string>
            {
                Messages.Places,
                Messages.Home,
                Messages.Field,
                Messages.River,
                Messages.Forest
            };
        }

        public override async Task ExecuteAsync(Update update)
        {
            await BotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Got it!");

            if (SelectedMessage.Equals(Messages.Places, System.StringComparison.InvariantCultureIgnoreCase))
            {
                await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
                await BotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "What would you like to see?",
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                        new [] { InlineKeyboardButton.WithCallbackData($"Home {Emojis.House}", Messages.Home) },
                        new [] { InlineKeyboardButton.WithCallbackData($"River {Emojis.WaterWave}", Messages.River) }, 
                        new [] { InlineKeyboardButton.WithCallbackData($"Forest {Emojis.Tree}", Messages.Forest) },
                        new [] { InlineKeyboardButton.WithCallbackData($"Field {Emojis.EarOfRice}", Messages.Field) }
                    }));
                return;
            }

            //update.CallbackQuery.Message.From.LanguageCode

            await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
            await BotClient.SendMediaGroupAsync(await GetMediaFromBlobByPrefix(SelectedMessage), update.CallbackQuery.Message.Chat.Id);
        }

        private async Task<IAlbumInputMedia[]> GetMediaFromBlobByPrefix(string mediaPrefix)
        {
            var mediaList = new List<IAlbumInputMedia>();
            var blobPages = _blobContainerClient.GetBlobsAsync(prefix: mediaPrefix).AsPages();

            await foreach (var blobPage in blobPages)
            {
                foreach (var blobItem in blobPage.Values)
                {
                    mediaList.Add(new InputMediaPhoto(_blobContainerClient.GetBlobClient(blobItem.Name).Uri.AbsoluteUri));
                }
            }
            return mediaList.ToArray();
        }
    }
}
