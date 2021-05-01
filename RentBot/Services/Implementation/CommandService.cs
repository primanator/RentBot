using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using RentBot.Commands;
using RentBot.Constants;
using RentBot.Factories;
using RentBot.Model;
using RentBot.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RentBot.Services.Implementation
{
    public class CommandService : ICommandService
    {
        private readonly LinkedCommand _rootCommand;
        private readonly LinkedCommand _defaultCommand;

        public CommandService()
        {
            _defaultCommand = new LinkedCommand(Messages.Default, StartFunc);
            _rootCommand = new LinkedCommand(string.Empty)
            {
                ChildCommands = new List<LinkedCommand>
                {
                    new LinkedCommand(Messages.FallBack, StartFunc),
                    new LinkedCommand(Messages.Start, StartFunc)
                    {
                        ChildCommands = new List<LinkedCommand>
                        {
                            new LinkedCommand(Messages.About, AboutFunc, OtherOptionsFallback),
                            new LinkedCommand(Messages.FAQ, FaqFunc, OtherOptionsFallback),
                            new LinkedCommand(Messages.Places, PlacesFunc)
                            {
                                ChildCommands = new List<LinkedCommand>
                                {
                                    new LinkedCommand(Messages.Home, BlobPhotoFunc, PlaceFallback),
                                    new LinkedCommand(Messages.Field, BlobPhotoFunc, PlaceFallback),
                                    new LinkedCommand(Messages.River, BlobPhotoFunc, PlaceFallback),
                                    new LinkedCommand(Messages.Forest, BlobPhotoFunc, PlaceFallback),
                                    new LinkedCommand(Messages.Food, FoodFunc)
                                    {
                                        ChildCommands = new List<LinkedCommand>
                                        {
                                            new LinkedCommand(Messages.Minimarket, BlobPhotoFunc, FoodFallback)
                                            {
                                                ChildCommands = new List<LinkedCommand> { new LinkedCommand(Messages.MinimarketLocation, MinimarketLocationFunc, PlaceFallback) }
                                            },
                                            new LinkedCommand(Messages.Supermarket, BlobPhotoFunc, FoodFallback)
                                            {
                                                ChildCommands = new List<LinkedCommand> { new LinkedCommand(Messages.SupermarketLocation, SupermarketLocationFunc, PlaceFallback) }
                                            },
                                            new LinkedCommand(Messages.Takeout, BlobPhotoFunc, FoodFallback)
                                            {
                                                ChildCommands = new List<LinkedCommand> { new LinkedCommand(Messages.TakeoutLocation, TakeoutLocationFunc, PlaceFallback) }
                                            }
                                        }
                                    }
                                }
                            },
                            new LinkedCommand(Messages.Path, PathFunc)
                            {
                                ChildCommands = new List<LinkedCommand>
                                {
                                    new LinkedCommand(Messages.BusSchedule, BusScheduleFunc)
                                    {
                                        ChildCommands = new List<LinkedCommand>
                                        {
                                            new LinkedCommand(Messages.ToCity, ToCityFunc, RouteFallback),
                                            new LinkedCommand(Messages.FromCity, FromCityFunc, RouteFallback)
                                        }
                                    },
                                    new LinkedCommand(Messages.HomeGeo, HomeGeoFunc)
                                }
                            }
                        }
                    },
                }
            };
        }

        public LinkedCommand GetCommandByMessage(string message)
        {
            var children = new Queue<LinkedCommand>();
            children.Enqueue(_rootCommand);

            while(children.Count != 0)
            {
                var node = children.Dequeue();

                if (node.CommandMessage.Equals(message, StringComparison.InvariantCultureIgnoreCase))
                {
                    return node;
                }
                node.ChildCommands.ForEach(childNode => children.Enqueue(childNode));
            }
            return _defaultCommand;
        }

        private async Task StartFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();
            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);

            if (!string.IsNullOrEmpty(request.CallbackQueryId)) // fallback query needs to be answered
            {
                await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            }

            string responseText;
            var name = request.User.FirstName + (string.IsNullOrEmpty(request.User.LastName) ? string.Empty : $" {request.User.LastName}");

            switch(request.Message)
            {
                case Messages.Start:
                    {
                        responseText = $"Hi, {name}!\nHow can I help you?";
                        break;
                    }
                case Messages.FallBack:
                    {
                        responseText = "Can I help you with anything else?";
                        break;
                    }
                default:
                    {
                        responseText = $"What we were talking about, {name}?";
                        break;
                    }
            }

            await botClient.SendTextMessageAsync(request.ChatId, responseText,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Maps {Emojis.Map}", Messages.Path) },
                    new [] { InlineKeyboardButton.WithCallbackData($"What's around? {Emojis.Eyes}", Messages.Places) },
                    new [] { InlineKeyboardButton.WithUrl($"Booking {Emojis.HouseWithGarden}", "https://abnb.me/Y5SjYolOneb") },
                    new [] { InlineKeyboardButton.WithCallbackData($"About us {Emojis.SpeechBalloon}", Messages.About) },
                    new [] { InlineKeyboardButton.WithCallbackData($"F.A.Q. {Emojis.PageWithCurl}", Messages.FAQ) }
                }));
        }

        private async Task PathFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();
            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "Pick your map!",
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Bus Schedule {Emojis.Minibus}", Messages.BusSchedule) },
                    new [] { InlineKeyboardButton.WithCallbackData($"Home Geolocation {Emojis.Pushpin}", Messages.HomeGeo) }
                }));
        }

        private async Task BusScheduleFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();
            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "Choose your direction!",
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"From Kyiv {Emojis.RightPointing}", Messages.FromCity) },
                    new [] { InlineKeyboardButton.WithCallbackData($"To Kyiv {Emojis.LeftPointing}", Messages.ToCity) }
                }));
        }

        private async Task HomeGeoFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();
            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "Here are the coordinates!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
            var latitude = float.Parse(Environment.GetEnvironmentVariable("LATITUDE", EnvironmentVariableTarget.Process));
            var longitude = float.Parse(Environment.GetEnvironmentVariable("LONGITUDE", EnvironmentVariableTarget.Process));
            await botClient.SendLocationAsync(request.ChatId, latitude, longitude);
        }

        private async Task ToCityFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();
            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            var schedule = "Station: Near the road\nBus #341: Rozhny-Kyiv\nDeparture time: 7:00, 8:10, 9:45, 10:50, 12:35, 15:50, 17:20, 19:10, 20:15";
            await botClient.SendTextMessageAsync(request.ChatId, schedule);

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
            await botClient.SendLocationAsync(request.ChatId, 50.6342762f, 30.7163217f);
        }

        private async Task FromCityFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();
            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            var schedule = "Station: AS Darnitsa\nBus #341: Kyiv-Rozhny\nDeparture time: 6:45, 8:00, 9:25, 11:05, 14:00, 15:25, 17:30, 18:45";
            await botClient.SendTextMessageAsync(request.ChatId, schedule);

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
            await botClient.SendLocationAsync(request.ChatId, 50.46053868555018f, 30.637056277607083f);
        }

        private async Task RouteFallback(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.SendTextMessageAsync(request.ChatId, "Have a nice trip!", replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"Okay! {Emojis.OkSign}", Messages.FallBack) }
            }));
        }

        private async Task FaqFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "To use this bot simply follow the commands appearing on the screen.\n\n" +
                "If you'd like to begin conversation with the bot from the start type /start.\n\n" +
                "Or choose it from the list of available commands in the bottom right corner with '/' button.");
        }

        private async Task AboutFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Nam pretium eros sit amet gravida aliquet. Ut ac laoreet est. " +
                "Vivamus non molestie est.\n\n" +
                "Telegram: @number716\n" +
                "Instagram: https://instagram.com/another__place_");
        }

        private async Task OtherOptionsFallback(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.SendTextMessageAsync(request.ChatId, "Check other options?", replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"Yeah! {Emojis.OkSign}", Messages.FallBack) }
            }));
        }

        private async Task PlacesFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "What would you like to see?",
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                        new [] { InlineKeyboardButton.WithCallbackData($"Home {Emojis.House}", Messages.Home) },
                        new [] { InlineKeyboardButton.WithCallbackData($"Food {Emojis.FrenchFries}", Messages.Food) },
                        new [] { InlineKeyboardButton.WithCallbackData($"River {Emojis.WaterWave}", Messages.River) },
                        new [] { InlineKeyboardButton.WithCallbackData($"Forest {Emojis.Tree}", Messages.Forest) },
                        new [] { InlineKeyboardButton.WithCallbackData($"Field {Emojis.EarOfRice}", Messages.Field) }
                }));
        }

        private async Task BlobPhotoFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendMediaGroupAsync(await GetMediaFromBlobByPrefix(clientFactory.GetBlobContainerClient(), request.Message), request.ChatId);
        }

        private async Task<IAlbumInputMedia[]> GetMediaFromBlobByPrefix(BlobContainerClient blobContainerClinet, string mediaPrefix)
        {
            var mediaList = new List<IAlbumInputMedia>();
            var blobPages = blobContainerClinet.GetBlobsAsync(prefix: mediaPrefix).AsPages();

            await foreach (var blobPage in blobPages)
            {
                foreach (var blobItem in blobPage.Values)
                {
                    mediaList.Add(new InputMediaPhoto(blobContainerClinet.GetBlobClient(blobItem.Name).Uri.AbsoluteUri));
                }
            }
            return mediaList.ToArray();
        }

        private async Task PlaceFallback(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.SendTextMessageAsync(request.ChatId, "Looks nice?", replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"Yeap! {Emojis.HeartEyes}", Messages.FallBack) }
            }));
        }

        private async Task FoodFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "Choose yours!",
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData($"Supermarket {Emojis.Store}", Messages.Supermarket) },
                    new[] { InlineKeyboardButton.WithCallbackData($"Minimarket {Emojis.PotOfFood}", Messages.Minimarket) },
                    new[] { InlineKeyboardButton.WithCallbackData($"Takeout {Emojis.Pizza}", Messages.Takeout) }
                }));
        }

        private async Task FoodFallback(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

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

            await botClient.SendTextMessageAsync(request.ChatId, "See location?", replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"Yes! {Emojis.DeliciousFace}", callbackCommand) }
            }));
        }

        private async Task MinimarketLocationFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "Minimarket is just near by!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
            await botClient.SendLocationAsync(request.ChatId, 50.6632402f, 30.7309756f);
        }

        private async Task SupermarketLocationFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "Supermarket is just near by!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
            await botClient.SendLocationAsync(request.ChatId, 50.6030410f, 30.7044328f);
        }

        private async Task TakeoutLocationFunc(IClientFactory clientFactory, TelegramRequest request)
        {
            var botClient = await clientFactory.GetTelegramBotClient();

            await botClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
            await botClient.SendTextMessageAsync(request.ChatId, "Takeout is just near by!");

            await botClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
            await botClient.SendLocationAsync(request.ChatId, 50.6781993f, 30.7388073f);
        }
    }
}
