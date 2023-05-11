using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RentBot.Model;
using RentBot.Services.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RentBot.Services.Implementation;

public class ModelConverterService : IModelConverterService
{
    private readonly ILogger<ModelConverterService> _logger;

    public ModelConverterService(ILogger<ModelConverterService> logger)
    {
        _logger = logger;
    }

    public TelegramRequest UpdateToTelegramRequest(Update update)
    {
        _logger.LogInformation("Update:\n" + update);

        return new TelegramRequest(update);
    }

    public async Task<Update> HttpRequestToTelegramUpdateAsync(HttpRequest httpRequest)
    {
        var requestStr = await httpRequest.ReadAsStringAsync();
        
        _logger.LogInformation("Http Request String\n:" + requestStr);

        return JsonConvert.DeserializeObject<Update>(requestStr);
    }
}
