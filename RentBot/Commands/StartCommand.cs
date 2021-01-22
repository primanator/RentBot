using System.Threading.Tasks;
using RentBot.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RentBot.Commands
{
    internal class StartCommand : AbstractCommand, ICommand
    {
        public override string Name => ListOfCommands.Start;

        public StartCommand(ITelegramBotClient botClient): base(botClient) { }

        public override async Task Execute(Update update)
        {
            await _botClient.SendChatActionAsync(update.Message.Chat.Id, ChatAction.Typing);

            if (update.Message.Text == Name) // TODO: remove redundant branch
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, GetWelcomingText(update.Message.From),
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                        new [] { InlineKeyboardButton.WithCallbackData("How do I get to your place?", ListOfCommands.Path) },
                        new [] { InlineKeyboardButton.WithCallbackData("Show me what's around!", ListOfCommands.Places) }
                }));
            }
            else
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat, update.Message.Text);
            }
        }

        private string GetWelcomingText(User user)
        {
            var welcomeText = $"Hi, {user.FirstName}";
            welcomeText += string.IsNullOrEmpty(user.LastName) ? string.Empty : $" {user.LastName}";
            return welcomeText + "!";
        }
    }
}
