using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using RentBot.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RentBot;

public class Functions
{
    private readonly IBotService _botService;

    public Functions(IBotService botService) => _botService = botService;

    [FunctionName("TelegramWebhookFunc")]
    public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger logger)
    {
        try
        {
            await _botService.ProcessAsync(httpRequest);
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(Functions)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
        }
    }
}
