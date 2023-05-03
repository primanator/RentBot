using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using RentBot.Services.Interfaces;
using System;

namespace RentBot.Services.Implementation;

public class BlobServiceClientWrapper : IBlobServiceClientWrapper
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _blobContainerName;
    private readonly ILogger _logger;

    public BlobServiceClientWrapper(string accountName, string blobContainerName, ILogger<BlobServiceClientWrapper> logger)
    {
        _blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), new DefaultAzureCredential());
        _blobContainerName = blobContainerName;
        _logger = logger;
    }

    public BlobContainerClient GetBlobContainerClient()
    {
        return _blobServiceClient.GetBlobContainerClient(_blobContainerName);
    }
}
