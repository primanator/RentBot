using System;
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
    internal class StartCmd : AbstractCmd, ICommand
    {
        public StartCmd(IClientFactory clientFactory, ILogger logger) : base(clientFactory, logger) { }

        public override async Task ExecuteAsync(Update update)
        {
            await BotClient.SendChatActionAsync(update.Message.Chat.Id, ChatAction.Typing);

            var responseText = GetText(update.Message.From, DetailedCommand.Equals(ListOfCommands.Start, StringComparison.InvariantCultureIgnoreCase));

            await BotClient.SendTextMessageAsync(update.Message.Chat.Id, responseText,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData($"Maps {Emojis.Map}", ListOfCommands.Path) },
                    new [] { InlineKeyboardButton.WithCallbackData($"What's around? {Emojis.Eyes}", ListOfCommands.Places) }
                }));
        }

        private string GetText(User user, bool isWelcome)
        {
            if (isWelcome)
            {
                return $"Hi, {user.FirstName}" + (string.IsNullOrEmpty(user.LastName) ? "!" : $" {user.LastName}!");
            }
            return $"{user.FirstName}, what we were talking about?";
        }
    }
}
