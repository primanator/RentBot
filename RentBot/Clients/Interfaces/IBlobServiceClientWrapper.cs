using Azure.Storage.Blobs;

namespace RentBot.Clients.Interfaces;

public interface IBlobServiceClientWrapper
{
    BlobContainerClient GetBlobContainerClient();
}
