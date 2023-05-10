using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RentBot.Model;
using RentBot.Services.Interfaces;

[assembly: InternalsVisibleTo("RentBot.Tests")]
namespace RentBot.Services.Implementation;

public class BotService : IBotService
{
    private readonly ICommandService _commandService;
    private readonly ILogger<BotService> _logger;

    public BotService(ICommandService commandService, ILogger<BotService> logger)
    {
        _commandService = commandService;
        _logger = logger;
    }

    public async Task ProcessAsync(Request request)
    {
        try
        {
            var command = await _commandService.GetCommandByMessage(request.Message);
            await command.Function(request);
            await command.Fallback(request);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(BotService)} encountered error: {ex.Message}\n StackTrace: {ex.StackTrace}");
        }
    }
}
