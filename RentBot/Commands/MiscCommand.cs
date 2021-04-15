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
    internal class MiscCommand : AbstractCommand, ICommand
    {
        public MiscCommand(IClientFactory clientFactory, ILogger logger) : base(clientFactory, logger)
        {
            AvailableMessages = new List<string>
            {
                Messages.About,
                Messages.FAQ,
                Messages.SupermarketLocation,
                Messages.MinimarketLocation,
                Messages.TakeoutLocation
            };
        }

        public override async Task ExecuteAsync(TelegramRequest request)
        {
            await BotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            var fallbackNeeded = false;
            var foodFallback = false;
            string responseMessage;

            switch (request.Message)
            {
                case Messages.About:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        responseMessage = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                            "Nam pretium eros sit amet gravida aliquet. Ut ac laoreet est. " +
                            "Vivamus non molestie est.\n\n" +
                            "Telegram: @number716\n" +
                            "Instagram: https://instagram.com/another__place_";
                        await BotClient.SendTextMessageAsync(request.ChatId, responseMessage);
                        fallbackNeeded = true;
                        break;
                    }
                case Messages.FAQ:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        responseMessage = "To use this bot simply follow the commands appearing on the screen.\n\n" +
                            "If you'd like to begin conversation with the bot from the start type /start.\n\n" +
                            "Or choose it from the list of available commands in the bottom right corner with '/' button.";
                        await BotClient.SendTextMessageAsync(request.ChatId, responseMessage);
                        fallbackNeeded = true;
                        break;
                    }
                case Messages.SupermarketLocation:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(request.ChatId, "Supermarket is just near by!");

                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
                        await BotClient.SendLocationAsync(request.ChatId, 50.6030410f, 30.7044328f);
                        fallbackNeeded = true;
                        foodFallback = true;
                        break;
                    }
                case Messages.MinimarketLocation:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(request.ChatId, "Minimarket is just near by!");

                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
                        await BotClient.SendLocationAsync(request.ChatId, 50.6632402f, 30.7309756f);
                        fallbackNeeded = true;
                        foodFallback = true;
                        break;
                    }
                case Messages.TakeoutLocation:
                    {
                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);
                        await BotClient.SendTextMessageAsync(request.ChatId, "Takeout is just near by!");

                        await BotClient.SendChatActionAsync(request.ChatId, ChatAction.FindLocation);
                        await BotClient.SendLocationAsync(request.ChatId, 50.6781993f, 30.7388073f);
                        fallbackNeeded = true;
                        foodFallback = true;
                        break;
                    }
                default:
                    {
                        break;
                    }
            };

            if (fallbackNeeded)
            {
                if (foodFallback)
                {
                    await FallbackAsync(request.ChatId, "Looks nice?", new InlineKeyboardMarkup(new[]
                    {
                        new [] { InlineKeyboardButton.WithCallbackData($"Yeap! {Emojis.HeartEyes}", Messages.FallBack) }
                    }));
                    return;
                }

                await FallbackAsync(request.ChatId, "Check other options?", new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Yeah! {Emojis.OkSign}", Messages.FallBack) }
                }));
            }
        }
    }
}
