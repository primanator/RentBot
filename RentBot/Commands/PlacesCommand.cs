using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RentBot.Commands
{
    internal class PlacesCommand: AbstractCommand, ICommand
    {
        public override string Name => ListOfCommands.Places;

        public PlacesCommand(ITelegramBotClient botClient) : base(botClient) { }

        public override async Task Execute(Update update)
        {
            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"places command: {update.CallbackQuery.Data}");
        }
    }
}
