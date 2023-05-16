using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using RentBot.Clients.Interfaces;
using System;

namespace RentBot.Clients.Implementation;

public class BlobServiceClientWrapper : IBlobServiceClientWrapper
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _blobContainerName;

    public BlobServiceClientWrapper(string accountName, string accountKey, string blobContainerName)
    {
        _blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), new StorageSharedKeyCredential(accountName, accountKey));
        _blobContainerName = blobContainerName;
    }

    public BlobContainerClient GetBlobContainerClient()
    {
        return _blobServiceClient.GetBlobContainerClient(_blobContainerName);
    }
}
