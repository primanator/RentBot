using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using System;
using RentBot.Model;
using RentBot.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RentBot;

public class Functions
{
    private readonly IBotService _botService;

    public Functions(IBotService botService) => _botService = botService;

    [FunctionName("TelegramWebhookFunc")]
    public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger logger)
    {
        try
        {
            var requestStr = await req.ReadAsStringAsync();

            logger.LogInformation("RequestStr: " + requestStr);

            var request = new Request(JsonConvert.DeserializeObject<Update>(requestStr));

            await _botService.ProcessAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(Functions)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
        }
    }
}
