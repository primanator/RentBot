using System.Threading.Tasks;
using RentBot.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Commands
{
    internal class PlacesCommand: AbstractCommand, ICommand
    {
        public override string Name => ListOfCommands.Places;

        public PlacesCommand(ITelegramBotClient botClient) : base(botClient) { }

        public override async Task Execute(Update update)
        {
            await _botClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);

            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"places command: {update.CallbackQuery.Data}");
        }
    }
}
