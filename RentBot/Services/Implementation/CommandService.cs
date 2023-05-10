using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RentBot.Commands;
using RentBot.Commands.Interfaces;
using RentBot.Constants;
using RentBot.Model;
using RentBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RentBot.Services.Implementation;

public class CommandService : ICommandService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IBlobServiceClientWrapper _blobServiceClientWrapper;
    private ILinkedCommand _rootCommand;

    private ILinkedCommand RootCommand
    {
        get
        {
            if (_rootCommand == default)
                _rootCommand = GetRootCommand();
            return _rootCommand;
        }
    }

    public CommandService(ITelegramBotClient telegramBotClient, IBlobServiceClientWrapper blobServiceClientWrapper)
    {
        _telegramBotClient = telegramBotClient;
        _blobServiceClientWrapper = blobServiceClientWrapper;
        _rootCommand = GetRootCommand();
    }

    public async Task<ILinkedCommand> GetCommandByMessage(string message)
    {
        await ConfigureTelegramBotClientCommands();

        var children = new Queue<ILinkedCommand>();
        children.Enqueue(RootCommand);

        while (children.Count != 0)
        {
            var node = children.Dequeue();

            if (node.CommandMessage.Equals(message, StringComparison.InvariantCultureIgnoreCase))
            {
                return node;
            }
            node.ChildCommands.ForEach(childNode => children.Enqueue(childNode));
        }
        return new LinkedCommand(Messages.Default, StartFunc);
    }

    private async Task ConfigureTelegramBotClientCommands()
    {
        var startCommand = new BotCommand { Command = "/start", Description = "to begin conversation" };
        var commands = await _telegramBotClient.GetMyCommandsAsync();

        if (!commands.Any(existingCommand => existingCommand.Command == startCommand.Command.Remove(0, 1))) // commands are saved without '/'symbol on server
        {
            await _telegramBotClient.SetMyCommandsAsync(new List<BotCommand> { startCommand });
        }
    }

    private ILinkedCommand GetRootCommand() =>
        new LinkedCommand(string.Empty)
        {
            ChildCommands = new List<ILinkedCommand>
            {
                new LinkedCommand(Messages.FallBack, StartFunc),
                new LinkedCommand(Messages.Start, StartFunc)
                {
                    ChildCommands = new List<ILinkedCommand>
                    {
                        new LinkedCommand(Messages.About, AboutFunc, OtherOptionsFallback),
                        new LinkedCommand(Messages.FAQ, FaqFunc, OtherOptionsFallback),
                        new LinkedCommand(Messages.Places, PlacesFunc)
                        {
                            ChildCommands = new List<ILinkedCommand>
                            {
                                new LinkedCommand(Messages.Home, BlobPhotoFunc, PlaceFallback),
                                new LinkedCommand(Messages.Field, BlobPhotoFunc, PlaceFallback),
                                new LinkedCommand(Messages.River, BlobPhotoFunc, PlaceFallback),
                                new LinkedCommand(Messages.Forest, BlobPhotoFunc, PlaceFallback),
                                new LinkedCommand(Messages.Food, FoodFunc)
                                {
                                    ChildCommands = new List<ILinkedCommand>
                                    {
                                        new LinkedCommand(Messages.Minimarket, BlobPhotoFunc, FoodFallback)
                                        {
                                            ChildCommands = new List<ILinkedCommand> { new LinkedCommand(Messages.MinimarketLocation, MinimarketLocationFunc, PlaceFallback) }
                                        },
                                        new LinkedCommand(Messages.Supermarket, BlobPhotoFunc, FoodFallback)
                                        {
                                            ChildCommands = new List<ILinkedCommand> { new LinkedCommand(Messages.SupermarketLocation, SupermarketLocationFunc, PlaceFallback) }
                                        },
                                        new LinkedCommand(Messages.Takeout, BlobPhotoFunc, FoodFallback)
                                        {
                                            ChildCommands = new List<ILinkedCommand> { new LinkedCommand(Messages.TakeoutLocation, TakeoutLocationFunc, PlaceFallback) }
                                        }
                                    }
                                }
                            }
                        },
                        new LinkedCommand(Messages.Path, PathFunc)
                        {
                            ChildCommands = new List<ILinkedCommand>
                            {
                                new LinkedCommand(Messages.BusSchedule, BusScheduleFunc)
                                {
                                    ChildCommands = new List<ILinkedCommand>
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

    private async Task StartFunc(Request request)
    {
        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);

        if (!string.IsNullOrEmpty(request.CallbackQueryId)) // fallback query needs to be answered
        {
            await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
        }

        string responseText;
        var name = request.User.FirstName + (string.IsNullOrEmpty(request.User.LastName) ? string.Empty : $" {request.User.LastName}");

        switch (request.Message)
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

        await _telegramBotClient.SendTextMessageAsync(request.ChatId, responseText,
            replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"Maps {Emojis.Map}", Messages.Path) },
                new [] { InlineKeyboardButton.WithCallbackData($"What's around? {Emojis.Eyes}", Messages.Places) },
                new [] { InlineKeyboardButton.WithUrl($"Booking {Emojis.HouseWithGarden}", "https://abnb.me/Y5SjYolOneb") },
                new [] { InlineKeyboardButton.WithCallbackData($"About us {Emojis.SpeechBalloon}", Messages.About) },
                new [] { InlineKeyboardButton.WithCallbackData($"F.A.Q. {Emojis.PageWithCurl}", Messages.FAQ) }
            }));
    }

    private async Task PathFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Pick your map!",
            replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"Bus Schedule {Emojis.Minibus}", Messages.BusSchedule) },
                new [] { InlineKeyboardButton.WithCallbackData($"Home Geolocation {Emojis.Pushpin}", Messages.HomeGeo) }
            }));
    }

    private async Task BusScheduleFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Choose your direction!",
            replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"From Kyiv {Emojis.RightPointing}", Messages.FromCity) },
                new [] { InlineKeyboardButton.WithCallbackData($"To Kyiv {Emojis.LeftPointing}", Messages.ToCity) }
            }));
    }

    private async Task HomeGeoFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Here are the coordinates!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
        var latitude = float.Parse(Environment.GetEnvironmentVariable("LATITUDE", EnvironmentVariableTarget.Process));
        var longitude = float.Parse(Environment.GetEnvironmentVariable("LONGITUDE", EnvironmentVariableTarget.Process));
        await _telegramBotClient.SendLocationAsync(request.ChatId, latitude, longitude);
    }

    private async Task ToCityFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        var schedule = "Station: Near the road\nBus #341: Rozhny-Kyiv\nDeparture time: 7:00, 8:10, 9:45, 10:50, 12:35, 15:50, 17:20, 19:10, 20:15";
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, schedule);

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
        await _telegramBotClient.SendLocationAsync(request.ChatId, 50.6342762f, 30.7163217f);
    }

    private async Task FromCityFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        var schedule = "Station: AS Darnitsa\nBus #341: Kyiv-Rozhny\nDeparture time: 6:45, 8:00, 9:25, 11:05, 14:00, 15:25, 17:30, 18:45";
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, schedule);

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
        await _telegramBotClient.SendLocationAsync(request.ChatId, 50.46053868555018f, 30.637056277607083f);
    }

    private async Task RouteFallback(Request request)
    {
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Have a nice trip!", replyMarkup: new InlineKeyboardMarkup(new[]
        {
            new [] { InlineKeyboardButton.WithCallbackData($"Okay! {Emojis.OkSign}", Messages.FallBack) }
        }));
    }

    private async Task FaqFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "To use this bot simply follow the commands appearing on the screen.\n\n" +
            "If you'd like to begin conversation with the bot from the start type /start.\n\n" +
            "Or choose it from the list of available commands in the bottom right corner with '/' button.");
    }

    private async Task AboutFunc(Request request)
    {
        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId,
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Nam pretium eros sit amet gravida aliquet. Ut ac laoreet est. " +
            "Vivamus non molestie est.\n\n" +
            "Telegram: @number716\n" +
            "Instagram: https://instagram.com/another__place_");
    }

    private async Task OtherOptionsFallback(Request request)
    {
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Check other options?", replyMarkup: new InlineKeyboardMarkup(new[]
        {
            new [] { InlineKeyboardButton.WithCallbackData($"Yeah! {Emojis.OkSign}", Messages.FallBack) }
        }));
    }

    private async Task PlacesFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "What would you like to see?",
            replyMarkup: new InlineKeyboardMarkup(new[]
            {
                    new [] { InlineKeyboardButton.WithCallbackData($"Home {Emojis.House}", Messages.Home) },
                    new [] { InlineKeyboardButton.WithCallbackData($"Food {Emojis.FrenchFries}", Messages.Food) },
                    new [] { InlineKeyboardButton.WithCallbackData($"River {Emojis.WaterWave}", Messages.River) },
                    new [] { InlineKeyboardButton.WithCallbackData($"Forest {Emojis.Tree}", Messages.Forest) },
                    new [] { InlineKeyboardButton.WithCallbackData($"Field {Emojis.EarOfRice}", Messages.Field) }
            }));
    }

    private async Task BlobPhotoFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendMediaGroupAsync(request.ChatId,
            await GetMediaFromBlobByPrefix(request.Message));
    }

    private async Task<IAlbumInputMedia[]> GetMediaFromBlobByPrefix(string mediaPrefix)
    {
        var mediaList = new List<IAlbumInputMedia>();
        var blobContainerClient = _blobServiceClientWrapper.GetBlobContainerClient();
        var blobPages = blobContainerClient.GetBlobsAsync(prefix: mediaPrefix).AsPages();

        await foreach (var blobPage in blobPages)
        {
            foreach (var blobItem in blobPage.Values)
            {
                mediaList.Add(new InputMediaPhoto(blobContainerClient.GetBlobClient(blobItem.Name).Uri.AbsoluteUri));
            }
        }
        return mediaList.ToArray();
    }

    private async Task PlaceFallback(Request request)
    {
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Looks nice?", replyMarkup: new InlineKeyboardMarkup(new[]
        {
            new [] { InlineKeyboardButton.WithCallbackData($"Yeap! {Emojis.HeartEyes}", Messages.FallBack) }
        }));
    }

    private async Task FoodFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Choose yours!",
            replyMarkup: new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData($"Supermarket {Emojis.Store}", Messages.Supermarket) },
                new[] { InlineKeyboardButton.WithCallbackData($"Minimarket {Emojis.PotOfFood}", Messages.Minimarket) },
                new[] { InlineKeyboardButton.WithCallbackData($"Takeout {Emojis.Pizza}", Messages.Takeout) }
            }));
    }

    private async Task FoodFallback(Request request)
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

        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "See location?", replyMarkup: new InlineKeyboardMarkup(new[]
        {
            new [] { InlineKeyboardButton.WithCallbackData($"Yes! {Emojis.DeliciousFace}", callbackCommand) }
        }));
    }

    private async Task MinimarketLocationFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Minimarket is just near by!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
        await _telegramBotClient.SendLocationAsync(request.ChatId, 50.6632402f, 30.7309756f);
    }

    private async Task SupermarketLocationFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Supermarket is just near by!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
        await _telegramBotClient.SendLocationAsync(request.ChatId, 50.6030410f, 30.7044328f);
    }

    private async Task TakeoutLocationFunc(Request request)
    {
        await _telegramBotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
        await _telegramBotClient.SendTextMessageAsync(request.ChatId, "Takeout is just near by!");

        await _telegramBotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
        await _telegramBotClient.SendLocationAsync(request.ChatId, 50.6781993f, 30.7388073f);
    }
}
