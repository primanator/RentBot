using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using System;
using RentBot.Services.Implementation;
using RentBot.Factories;
using RentBot.Model;

namespace RentBot
{
    public static class TelegramWebhookFunc
    {
        [FunctionName("TelegramWebhookFunc")]
        public static async void Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var requestStr = await req.ReadAsStringAsync();
                log.LogInformation("RequestStr: " + requestStr);
                var request = new TelegramRequest(JsonConvert.DeserializeObject<Update>(requestStr));
                await new BotService(new ClientFactory(), log).ProcessAsync(request);
            }
            catch(Exception ex)
            {
                log.LogError($"{nameof(TelegramWebhookFunc)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
            }
        }
    }
}
