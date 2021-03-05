using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    internal class StartCommand : AbstractCommand, ICommand
    {
        public StartCommand(IClientFactory clientFactory, ILogger logger) : base(clientFactory, logger)
        {
            AvailableMessages = new List<string>
            {
                Messages.Start,
                Messages.Default,
                Messages.FallBack
            };
        }

        public override async Task ExecuteAsync(TelegramRequest request)
        {
            if (!string.IsNullOrEmpty(request.CallbackQueryId))
            {
                await BotClient.AnswerCallbackQueryAsync(request.CallbackQueryId, "Got it!");
            }

            await BotClient.SendChatActionAsync(request.ChatId, ChatAction.Typing);

            var response = GetResponse(request);

            await BotClient.SendTextMessageAsync(request.ChatId, response,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Maps {Emojis.Map}", Messages.Path) },
                    new [] { InlineKeyboardButton.WithCallbackData($"What's around? {Emojis.Eyes}", Messages.Places) }
                }));
        }

        private string GetResponse(TelegramRequest request)
        {
            var name = request.User.FirstName + (string.IsNullOrEmpty(request.User.LastName) ? string.Empty : $" {request.User.LastName}");
            if (request.Message.Equals(Messages.Start, StringComparison.InvariantCultureIgnoreCase))
            {
                return $"Hi, {name}!\nHow can I help you?";
            } else if (request.Message.Equals(Messages.FallBack, StringComparison.InvariantCultureIgnoreCase))
            {
                return "Can I help you with anything else?";
            }
            return $"What we were talking about, {name}?";
        }
    }
}
