using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentBot.Services.Implementation;
using RentBot.Services.Interfaces;
using Telegram.Bot;

[assembly: FunctionsStartup(typeof(RentBot.Startup))]
namespace RentBot;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;

        var blobAccountName = configuration["BLOB_ACCOUNT_NAME"];
        var blobContainerName = configuration["BLOB_CONTAINER_NAME"];

        builder.Services
               .AddSingleton<ITelegramBotClient>(serviceProvider => GetTelegramBotClient(configuration))
               .AddSingleton<ICommandService, CommandService>()
               .AddSingleton<IBotService, BotService>()
               .AddLogging()
               .AddSingleton<IBlobServiceClientWrapper>(serviceProvider =>
                   ActivatorUtilities.CreateInstance<BlobServiceClientWrapper>(serviceProvider, blobAccountName, blobContainerName));
    }

    private static TelegramBotClient GetTelegramBotClient(IConfiguration configuration)
    {
        var botSecret = configuration["BOT_SECRET"];

        return new TelegramBotClient(botSecret);
    }
}
