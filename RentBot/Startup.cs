using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentBot.Services.Implementation;
using RentBot.Services.Interfaces;
using Telegram.Bot;

namespace RentBot;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.GetContext().Configuration;
        var blobContainerClient = GetBlobContainerClient(configuration);

        var blobAccountName = configuration["BLOB_ACCOUNT_NAME"];
        var blobContainerName = configuration["BLOB_CONTAINER_NAME"];

        services
            .AddSingleton(blobContainerClient)
            .AddSingleton<ITelegramBotClient>(GetTelegramBotClient(configuration))
            .AddSingleton<ICommandService, CommandService>()
            .AddSingleton<IBotService, BotService>()
            .AddLogging()
            .AddSingleton<IBlobServiceClientWrapper>(serviceProvider =>
                ActivatorUtilities.CreateInstance<BlobServiceClientWrapper>(serviceProvider, blobAccountName, blobContainerClient));
    }

    private static BlobContainerClient GetBlobContainerClient(IConfiguration configuration)
    {
        var connectionString = configuration["STORAGE_CONNECTION_STR"];
        var containerName = configuration["CONTAINER_NAME"];

        return new BlobContainerClient(connectionString, containerName);
    }

    private static TelegramBotClient GetTelegramBotClient(IConfiguration configuration)
    {
        var botSecret = configuration["BOT_SECRET"];

        return new TelegramBotClient(botSecret);
    }
}
