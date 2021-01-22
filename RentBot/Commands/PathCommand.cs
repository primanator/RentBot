using System.Threading.Tasks;
using RentBot.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Commands
{
    internal class PathCommand: AbstractCommand, ICommand
    {
        public override string Name => ListOfCommands.Path;

        public PathCommand(ITelegramBotClient botClient) : base(botClient) { }

        public override async Task Execute(Update update)
        {
            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);

            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"path command: {update.CallbackQuery.Data}");
        }
    }
}
