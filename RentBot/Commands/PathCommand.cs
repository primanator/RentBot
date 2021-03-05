using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Constants;
using RentBot.Factories;
using RentBot.Model;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Emojis = RentBot.Constants.Emojis;

namespace RentBot.Commands
{
    internal class PathCommand: AbstractCommand, ICommand
    {
        public PathCommand(IClientFactory clientFactory, ILogger logger) : base(clientFactory, logger)
        {
            AvailableMessages = new List<string>
            {
                Messages.Path,
                Messages.BusSchedule,
                Messages.FromCity,
                Messages.ToCity,
                Messages.HomeGeo
            };
        }

        public override async Task ExecuteAsync(TelegramRequest request)
        {
            await BotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            var fallbackNeeded = false;

            switch (request.Message)
            {
                case Messages.BusSchedule:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(request.ChatId, "Choose your direction!",
                            replyMarkup: new InlineKeyboardMarkup(new[]
                            {
                                new [] { InlineKeyboardButton.WithCallbackData($"From Kyiv {Emojis.RightPointing}", Messages.FromCity) },
                                new [] { InlineKeyboardButton.WithCallbackData($"To Kyiv {Emojis.LeftPointing}", Messages.ToCity) }
                            }));
                        break;
                    }
                case Messages.FromCity:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        var schedule = "Station: AS Darnitsa\nBus #341: Kyiv-Rozhny\nDeparture time: 6:45, 8:00, 9:25, 11:05, 14:00, 15:25, 17:30, 18:45";
                        await BotClient.SendTextMessageAsync(request.ChatId, schedule);

                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
                        await BotClient.SendLocationAsync(request.ChatId, 50.46053868555018f, 30.637056277607083f);

                        fallbackNeeded = true;
                        break;
                    }
                case Messages.ToCity:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        var schedule = "Station: Near the road\nBus #341: Rozhny-Kyiv\nDeparture time: 7:00, 8:10, 9:45, 10:50, 12:35, 15:50, 17:20, 19:10, 20:15";
                        await BotClient.SendTextMessageAsync(request.ChatId, schedule);

                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
                        await BotClient.SendLocationAsync(request.ChatId, 50.6342762f, 30.7163217f);

                        fallbackNeeded = true;
                        break;
                    }
                case Messages.HomeGeo:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(request.ChatId, "Here are the coordinates!");

                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
                        var latitude = float.Parse(Environment.GetEnvironmentVariable("LATITUDE", EnvironmentVariableTarget.Process));
                        var longitude = float.Parse(Environment.GetEnvironmentVariable("LONGITUDE", EnvironmentVariableTarget.Process));
                        await BotClient.SendLocationAsync(request.ChatId, latitude, longitude);

                        fallbackNeeded = true;
                        break;
                    }
                default:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(request.ChatId, "Pick your map!",
                            replyMarkup: new InlineKeyboardMarkup(new[]
                            {
                                new [] { InlineKeyboardButton.WithCallbackData($"Bus Schedule {Emojis.Minibus}", Messages.BusSchedule) },
                                new [] { InlineKeyboardButton.WithCallbackData($"Home Geolocation {Emojis.Pushpin}", Messages.HomeGeo) }
                            }));
                        break;
                    }
            };

            if (fallbackNeeded)
            {
                await FallbackAsync(request.ChatId, "Have a nice trip!", new InlineKeyboardMarkup(new []
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Okay! {Emojis.OkSign}", Messages.FallBack) }
                }));
            }
        }
    }
}
