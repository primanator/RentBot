using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot;
using System;
using RentBot.Services.Implementation;
using RentBot.Factory;

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
                var telegramClient = new TelegramBotClient(botSecret);
                var commandService = new CommandService(telegramClient);
                var handlerFactory = new HandlerFactory(commandService, log);
                var botService = new BotService(handlerFactory, telegramClient, log);
                await botService.ProcessAsync(updateMessage);
            }
            catch(Exception ex)
            {
                log.LogError($"{nameof(TelegramWebhookFunc)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
            }
        }
    }
}
