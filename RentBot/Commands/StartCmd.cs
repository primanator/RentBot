using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
                    new [] { InlineKeyboardButton.WithCallbackData("How do I get to your place?", ListOfCommands.Path) },
                    new [] { InlineKeyboardButton.WithCallbackData("Show me what's around!", ListOfCommands.Places) }
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
