using System.Threading.Tasks;
using Azure.Storage.Blobs;
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
    internal class PathCmd: AbstractCmd, ICommand
    {
        private readonly BlobContainerClient _blobContainerClient;

        public PathCmd(IClientFactory clientFactory, ILogger logger) : base(clientFactory, logger)
        {
            _blobContainerClient = clientFactory.GetBlobContainerClient();
        }

        public override async Task ExecuteAsync(Update update)
        {
            await BotClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Got it!");
            
            if (DetailedCommand.Equals(ListOfCommands.BusSchedule, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var uri = _blobContainerClient.GetBlobClient("bus_schedule.jpg").Uri.AbsoluteUri;

                await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.UploadPhoto);
                await BotClient.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, uri);
            }
            else if (DetailedCommand.Equals(ListOfCommands.HomeGeo, System.StringComparison.InvariantCultureIgnoreCase))
            {
                await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.FindLocation);
                await BotClient.SendLocationAsync(update.CallbackQuery.Message.Chat.Id, 50.6351221f, 30.7111802f);
            }
            else
            {
                await BotClient.SendChatActionAsync(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
                await BotClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Pick your map!",
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                    new [] { InlineKeyboardButton.WithCallbackData($"Bus schedule {Emojis.Minibus}", ListOfCommands.BusSchedule) },
                    new [] { InlineKeyboardButton.WithCallbackData($"Geolocation {Emojis.Pushpin}", ListOfCommands.HomeGeo) }
                    }));
            }
        }
    }
}
