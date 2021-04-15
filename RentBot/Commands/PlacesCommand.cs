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
                Messages.Forest,
                Messages.Minimarket,
                Messages.Supermarket,
                Messages.Takeout
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
                        new [] { InlineKeyboardButton.WithCallbackData($"Field {Emojis.EarOfRice}", Messages.Field) },
                        new [] { InlineKeyboardButton.WithCallbackData($"Supermarket {Emojis.Store}", Messages.Supermarket) },
                        new [] { InlineKeyboardButton.WithCallbackData($"Minimarket {Emojis.PotOfFood}", Messages.Minimarket) },
                        new [] { InlineKeyboardButton.WithCallbackData($"Takeout {Emojis.Pizza}", Messages.Takeout) }
                    }));
                return;
            }

            await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await BotClient.SendMediaGroupAsync(await GetMediaFromBlobByPrefix(request.Message), request.ChatId);

            if (request.Message.Equals(Messages.Minimarket, System.StringComparison.InvariantCultureIgnoreCase)
                || request.Message.Equals(Messages.Supermarket, System.StringComparison.InvariantCultureIgnoreCase)
                || request.Message.Equals(Messages.Takeout, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var callbackCommand = string.Empty;

                switch (request.Message)
                {
                    case Messages.Minimarket:
                        {
                            callbackCommand = Messages.MinimarketLocation;
                            break;
                        }
                    case Messages.Supermarket:
                        {
                            callbackCommand = Messages.SupermarketLocation;
                            break;
                        }
                    case Messages.Takeout:
                        {
                            callbackCommand = Messages.TakeoutLocation;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                await FallbackAsync(request.ChatId, "See location?", new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Yes! {Emojis.DeliciousFace}", callbackCommand) }
                }));
                return;
            }

            await FallbackAsync(request.ChatId, "Looks nice?", new InlineKeyboardMarkup(new []
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Yeap! {Emojis.HeartEyes}", Messages.FallBack) }
                }));
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
