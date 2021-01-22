using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RentBot.Commands
{
    internal class PathCommand: AbstractCommand, ICommand
    {
        public override string Name => ListOfCommands.Path;

        public PathCommand(ITelegramBotClient botClient) : base(botClient) { }

        public override async Task Execute(Update update)
        {
            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"path command: {update.CallbackQuery.Data}");
        }
    }
}
