using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot;
using System;

namespace RentBot
{
    public static class TelegramWebhookFunc
    {
        [FunctionName("TelegramWebhookFunc")]
        public static async void Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            var requestBody = await req.ReadAsStringAsync();
            var updateMessage = JsonConvert.DeserializeObject<Update>(requestBody);
            var botSecret = Environment.GetEnvironmentVariable("BOT_SECRET", EnvironmentVariableTarget.Process);
            var botClient = new BotClient(new TelegramBotClient(botSecret), log);
            botClient.HandleUpdate(updateMessage);
        }
    }
}
