using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RentBot.Services.Interfaces;

namespace RentBot.Services.Implementation;

public class BotService : IBotService
{
    private readonly ICommandService _commandService;
    private readonly IModelConverterService _modelConverterService;
    private readonly ILogger<BotService> _logger;

    public BotService(ICommandService commandService, IModelConverterService modelConverterService, ILogger<BotService> logger)
    {
        _commandService = commandService;
        _modelConverterService = modelConverterService;
        _logger = logger;
    }

    public async Task ProcessAsync(HttpRequest httpRequest)
    {
        try
        {
            var update = await _modelConverterService.HttpRequestToTelegramUpdateAsync(httpRequest);
            var request = _modelConverterService.UpdateToTelegramRequest(update);
            var command = await _commandService.GetCommandByMessageAsync(request.Message);
            await command.Function(request);
            await command.Fallback(request);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(BotService)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
        }
    }
}
