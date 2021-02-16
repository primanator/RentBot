using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RentBot.Commands
{
    internal class StartCmd : ICommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public StartCmd(IClientFactory clientFactory, ILogger logger)
        {
            _botClient = clientFactory.GetTelegramBotClient();
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            await _botClient.SendChatActionAsync(update.Message.Chat.Id, ChatAction.Typing);

            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, GetWelcomingText(update.Message.From),
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new [] { InlineKeyboardButton.WithCallbackData("How do I get to your place?", ListOfCommands.Path) },
                    new [] { InlineKeyboardButton.WithCallbackData("Show me what's around!", ListOfCommands.Places) }
                }));
        }

        private string GetWelcomingText(User user)
        {
            var welcomeText = $"Hi, {user.FirstName}";
            welcomeText += string.IsNullOrEmpty(user.LastName) ? string.Empty : $" {user.LastName}";
            return welcomeText + "!";
        }
    }
}
