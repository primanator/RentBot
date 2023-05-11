using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentBot.Clients.Implementation;
using RentBot.Clients.Interfaces;
using RentBot.Services.Implementation;
using RentBot.Services.Interfaces;
using System;
using Telegram.Bot;

[assembly: FunctionsStartup(typeof(RentBot.Startup))]
namespace RentBot;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;

        builder.Services
               .AddSingleton<IBlobServiceClientWrapper>(serviceProvider => GetBlobServiceClientWrapper(serviceProvider, configuration))
               .AddSingleton<ITelegramBotClient>(serviceProvider => GetTelegramBotClient(configuration))
               .AddSingleton<IModelConverterService, ModelConverterService>()
               .AddSingleton<ICommandService, CommandService>()
               .AddSingleton<IBotService, BotService>()
               .AddLogging();
    }

    private static TelegramBotClient GetTelegramBotClient(IConfiguration configuration)
    {
        var botSecret = configuration["BOT_SECRET"];

        return new TelegramBotClient(botSecret);
    }

    private static BlobServiceClientWrapper GetBlobServiceClientWrapper(IServiceProvider serviceProvider, IConfiguration configuration)
    {

        var blobAccountName = configuration["BLOB_ACCOUNT_NAME"];
        var blobContainerName = configuration["BLOB_CONTAINER_NAME"];

        return ActivatorUtilities.CreateInstance<BlobServiceClientWrapper>(serviceProvider, blobAccountName, blobContainerName);
    }
}
