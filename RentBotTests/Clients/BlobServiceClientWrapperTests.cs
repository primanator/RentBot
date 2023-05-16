using NUnit.Framework;
using System;
using RentBot.Clients.Implementation;

namespace RentBot.Tests.Clients;

[TestFixture]
public class BlobServiceClientWrapperTests
{
    private string _accountName;
    private string _accountKey;
    private string _blobContainerName;

    [SetUp]
    public void SetUp()
    {
        _accountName = "accountName";
        _accountKey = "QWKTNwK3deyklb0TCyT0fCehqaHLnTdt7AORaX5DnQ1tQL3fXbRZWuUlSyejNeskqnPA+VdYfGjRNmPP2rlLCA==";
        _blobContainerName = "blobContainerName";
    }

    [TearDown]
    public void TearDown()
    {
        _accountName = null;
        _accountKey = null;
        _blobContainerName = null;
    }

    [Test]
    public void Constructor_ThrowsUriFormatException_WhenAccountNameIsNull()
    {
        Assert.Throws<UriFormatException>(() => new BlobServiceClientWrapper(null, _accountKey, _blobContainerName));
    }

    [Test]
    public void Constructor_ThrowsArgumentNullException_WhenAccountKeyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new BlobServiceClientWrapper(_accountName, null, _blobContainerName));
    }

    [Test]
    public void Constructor_CreatesBlobServiceClient_WhenParametersAreValid()
    {
        var blobServiceClientWrapper = new BlobServiceClientWrapper(_accountName, _accountKey, _blobContainerName);

        Assert.That(blobServiceClientWrapper, Is.Not.Null);
    }

    [Test]
    public void GetBlobContainerClient_ReturnsBlobContainerClient_WhenCalled()
    {
        var blobServiceClientWrapper = new BlobServiceClientWrapper(_accountName, _accountKey, _blobContainerName);

        var blobContainerClient = blobServiceClientWrapper.GetBlobContainerClient();

        Assert.That(blobContainerClient, Is.Not.Null);
    }

    [Test]
    public void GetBlobContainerClient_ReturnsBlobContainerClientWithCorrectName_WhenCalled()
    {
        var blobServiceClientWrapper = new BlobServiceClientWrapper(_accountName, _accountKey, _blobContainerName);

        var blobContainerClient = blobServiceClientWrapper.GetBlobContainerClient();

        Assert.That(blobContainerClient.Name, Is.EqualTo(_blobContainerName));
    }

    [Test]
    public void GetBlobContainerClient_ReturnsBlobContainerClientWithCorrectUri_WhenCalled()
    {
        var blobServiceClientWrapper = new BlobServiceClientWrapper(_accountName, _accountKey, _blobContainerName);

        var blobContainerClient = blobServiceClientWrapper.GetBlobContainerClient();

        Assert.That(blobContainerClient.Uri, Is.EqualTo(new Uri($"https://{_accountName}.blob.core.windows.net/{_blobContainerName}")));
    }
}
