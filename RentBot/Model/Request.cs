using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RentBot.Model;

public class Request
{
    public long ChatId { get; private set; }
    public string CallbackQueryId { get; private set; }
    public User User { get; private set; }
    public string Message { get; private set; }

    public Request(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                {
                    Message = update.Message.Text;
                    ChatId = update.Message.Chat.Id;
                    User = update.Message.From;
                    break;
                }
            case UpdateType.CallbackQuery:
                {
                    Message = update.CallbackQuery.Data;
                    ChatId = update.CallbackQuery.Message.Chat.Id;
                    User = update.CallbackQuery.Message.From;
                    CallbackQueryId = update.CallbackQuery.Id;
                    break;

                }
            default:
                {
                    throw new ArgumentOutOfRangeException($"{nameof(Request)} can't decompose update type {update.Type}!");
                }
        }
    }
}
