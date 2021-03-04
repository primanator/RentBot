using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Constants;
using RentBot.Factories;
using RentBot.Model;
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

        public override async Task ExecuteAsync(TelegramRequest request)
        {
            await BotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

            if (request.Message.Equals(Messages.Places, System.StringComparison.InvariantCultureIgnoreCase))
            {
                await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                await BotClient.SendTextMessageAsync(request.ChatId, "What would you like to see?",
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

            await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await BotClient.SendMediaGroupAsync(await GetMediaFromBlobByPrefix(request.Message), request.ChatId);

            await FallbackAsync(request.ChatId, "Looks nice!", $"Yeap! {Emojis.HeartEyes}");
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
