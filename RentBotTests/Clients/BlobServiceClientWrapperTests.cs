using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using System;
using RentBot.Clients.Implementation;

namespace RentBot.Tests.Clients;

[TestFixture]
public class BlobServiceClientWrapperTests
{
    private Mock<ILogger<BlobServiceClientWrapper>> _loggerMock;
    private string _accountName;
    private string _blobContainerName;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<BlobServiceClientWrapper>>();
        _accountName = "accountName";
        _blobContainerName = "blobContainerName";
    }

    [TearDown]
    public void TearDown()
    {
        _loggerMock = null;
        _accountName = null;
        _blobContainerName = null;
    }

    [Test]
    public void Constructor_ThrowsArgumentNullException_WhenAccountNameIsNull()
    {
        Assert.Throws<UriFormatException>(() => new BlobServiceClientWrapper(null, _blobContainerName, _loggerMock.Object));
    }

    [Test]
    public void Constructor_CreatesBlobServiceClient_WhenParametersAreValid()
    {
        var blobServiceClientWrapper = new BlobServiceClientWrapper(_accountName, _blobContainerName, _loggerMock.Object);

        Assert.That(blobServiceClientWrapper, Is.Not.Null);
    }

    [Test]
    public void GetBlobContainerClient_ReturnsBlobContainerClient_WhenCalled()
    {
        var blobServiceClientWrapper = new BlobServiceClientWrapper(_accountName, _blobContainerName, _loggerMock.Object);

        var blobContainerClient = blobServiceClientWrapper.GetBlobContainerClient();

        Assert.That(blobContainerClient, Is.Not.Null);
    }

    [Test]
    public void GetBlobContainerClient_ReturnsBlobContainerClientWithCorrectName_WhenCalled()
    {
        var blobServiceClientWrapper = new BlobServiceClientWrapper(_accountName, _blobContainerName, _loggerMock.Object);

        var blobContainerClient = blobServiceClientWrapper.GetBlobContainerClient();

        Assert.That(blobContainerClient.Name, Is.EqualTo(_blobContainerName));
    }

    [Test]
    public void GetBlobContainerClient_ReturnsBlobContainerClientWithCorrectUri_WhenCalled()
    {
        var blobServiceClientWrapper = new BlobServiceClientWrapper(_accountName, _blobContainerName, _loggerMock.Object);

        var blobContainerClient = blobServiceClientWrapper.GetBlobContainerClient();

        Assert.That(blobContainerClient.Uri, Is.EqualTo(new Uri($"https://{_accountName}.blob.core.windows.net/{_blobContainerName}")));
    }
}
