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
        public static async void Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var requestBody = await req.ReadAsStringAsync();
                log.LogInformation("requestbody: " + requestBody);
                var updateMessage = JsonConvert.DeserializeObject<Update>(requestBody);
                var botSecret = Environment.GetEnvironmentVariable("BOT_SECRET", EnvironmentVariableTarget.Process);
                new BotClient(new TelegramBotClient(botSecret), log).Process(updateMessage);
            }
            catch(Exception ex)
            {
                log.LogError("webhook func error: " + ex.Message);
            }
        }
    }
}
