using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public override async Task ExecuteAsync(Update update)
        {
            await BotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Got it!");
            
            switch (SelectedMessage)
            {
                case Messages.BusSchedule:
                    {
                        await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Choose your direction!",
                            replyMarkup: new InlineKeyboardMarkup(new[]
                            {
                                new [] { InlineKeyboardButton.WithCallbackData($"From Kyiv {Emojis.RightPointing}", Messages.ToCity) },
                                new [] { InlineKeyboardButton.WithCallbackData($"To Kyiv {Emojis.LeftPointing}", Messages.FromCity) }
                            }));
                        break;
                    }
                case Messages.FromCity:
                    {
                        await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
                        var schedule = "Station: AC Darnitsa\nBus #341: Kyiv-Rozhny\nDeparture time: 6:45, 8:00, 9:25, 11:05, 14:00, 15:25, 17:30, 18:45";
                        await BotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, schedule);
                        await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.FindLocation);
                        await BotClient.SendLocationAsync(update.CallbackQuery.Message.Chat.Id, 50.46053868555018f, 30.637056277607083f);
                        break;
                    }
                case Messages.ToCity:
                    {
                        await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
                        var schedule = "Station: Near the road\nBus #341: Rozhny-Kyiv\nDeparture time: 7:00, 8:10, 9:45, 10:50, 12:35, 15:50, 17:20, 19:10, 20:15";
                        await BotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, schedule);

                        await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.FindLocation);
                        var latitude = float.Parse(Environment.GetEnvironmentVariable("LATITUDE", EnvironmentVariableTarget.Process));
                        var longitude = float.Parse(Environment.GetEnvironmentVariable("LONGITUDE", EnvironmentVariableTarget.Process));
                        await BotClient.SendLocationAsync(update.CallbackQuery.Message.Chat.Id, latitude, longitude);
                        break;
                    }
                case Messages.HomeGeo:
                    {
                        await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Here are the coordinates!");
                        await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.FindLocation);
                        await BotClient.SendLocationAsync(update.CallbackQuery.Message.Chat.Id, 50.6351221f, 30.7111802f);
                        break;
                    }
                default:
                    {
                        await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Pick your map!",
                            replyMarkup: new InlineKeyboardMarkup(new[]
                            {
                                new [] { InlineKeyboardButton.WithCallbackData($"Bus Schedule {Emojis.Minibus}", Messages.BusSchedule) },
                                new [] { InlineKeyboardButton.WithCallbackData($"Home Geolocation {Emojis.Pushpin}", Messages.HomeGeo) }
                            }));
                        break;
                    }
            };
        }
    }
}
