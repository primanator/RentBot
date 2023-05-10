using Azure.Storage.Blobs;

namespace RentBot.Services.Interfaces;

public interface IBlobServiceClientWrapper
{
    BlobContainerClient GetBlobContainerClient();
}
