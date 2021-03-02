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
    internal class StartCommand : AbstractCommand, ICommand
    {
        public StartCommand(IClientFactory clientFactory, ILogger logger) : base(clientFactory, logger)
        {
            AvailableMessages = new List<string>
            {
                Messages.Start,
                Messages.Default
            };
        }

        public override async Task ExecuteAsync(Update update)
        {
            await BotClient.SendChatActionAsync(update.Message.Chat.Id, ChatAction.Typing);

            var response = GetResponse(update.Message.From, SelectedMessage.Equals(Messages.Start, StringComparison.InvariantCultureIgnoreCase));

            await BotClient.SendTextMessageAsync(update.Message.Chat.Id, response,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Maps {Emojis.Map}", Messages.Path) },
                    new [] { InlineKeyboardButton.WithCallbackData($"What's around? {Emojis.Eyes}", Messages.Places) }
                }));
        }

        private string GetResponse(User user, bool isWelcome)
        {
            if (isWelcome)
            {
                return $"Hi, {user.FirstName}" + (string.IsNullOrEmpty(user.LastName) ? "!" : $" {user.LastName}!\nHow can I help you?");
            }
            return $"{user.FirstName}, what we were talking about?";
        }
    }
}
