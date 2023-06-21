using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RentBot.Services.Interfaces;
using RentBot.Services.Implementation;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using System;
using RentBot.Models;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using RentBot.Commands;
using RentBot.Constants;
using Microsoft.AspNetCore.Http;

namespace RentBot.Tests.Services;

[TestFixture]
public class BotServiceTests
{
    private Mock<ICommandService> _commandServiceMock;
    private Mock<IModelConverterService> _modelConverterServiceMock;
    private Mock<ILogger<BotService>> _loggerMock;
    private Mock<HttpRequest> _httpRequestMock;
    private Mock<Func<TelegramRequest, Task>> _functionMock;
    private Mock<Func<TelegramRequest, Task>> _fallbackMock;
    private BotService _botService;
    private Update _updatePayload;
    private TelegramRequest _telegramRequest;

    [SetUp]
    public void SetUp()
    {
        _commandServiceMock = new Mock<ICommandService>();
        _modelConverterServiceMock = new Mock<IModelConverterService>();
        _loggerMock = new Mock<ILogger<BotService>>();
        _httpRequestMock = new Mock<HttpRequest>();
        _functionMock = new Mock<Func<TelegramRequest, Task>>();
        _fallbackMock = new Mock<Func<TelegramRequest, Task>>();
        _botService = new BotService(_commandServiceMock.Object, _modelConverterServiceMock.Object, _loggerMock.Object);
        _updatePayload = GenerateUpdatePayload();
        _telegramRequest = new TelegramRequest(_updatePayload);
    }

    [TearDown]
    public void TearDown()
    {
        _commandServiceMock = null;
        _modelConverterServiceMock = null;
        _loggerMock = null;
        _httpRequestMock = null;
        _functionMock = null;
        _fallbackMock = null;
        _botService = null;
        _updatePayload = null;
        _telegramRequest = null;
    }

    [Test]
    public async Task ProcessAsync_CallsGetCommandByMessage_ByDefault()
    {
        var request = new TelegramRequest(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessageAsync(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start));
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.HttpRequestToTelegramUpdateAsync(It.IsAny<HttpRequest>()))
            .ReturnsAsync(_updatePayload);
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.UpdateToTelegramRequest(It.IsAny<Update>()))
            .Returns(_telegramRequest);

        await _botService.ProcessAsync(_httpRequestMock.Object);

        _commandServiceMock.Verify(commnadService => commnadService.GetCommandByMessageAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_CallsCommandFunction_ByDefault()
    {
        var request = new TelegramRequest(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessageAsync(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start, _functionMock.Object, _fallbackMock.Object));
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.HttpRequestToTelegramUpdateAsync(It.IsAny<HttpRequest>()))
            .ReturnsAsync(_updatePayload);
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.UpdateToTelegramRequest(It.IsAny<Update>()))
            .Returns(_telegramRequest);

        await _botService.ProcessAsync(_httpRequestMock.Object);

        _functionMock.Verify(function => function(It.IsAny<TelegramRequest>()), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_CallsCommandFallback_ByDefault()
    {
        var request = new TelegramRequest(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessageAsync(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start, _functionMock.Object, _fallbackMock.Object));
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.HttpRequestToTelegramUpdateAsync(It.IsAny<HttpRequest>()))
            .ReturnsAsync(_updatePayload);
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.UpdateToTelegramRequest(It.IsAny<Update>()))
            .Returns(_telegramRequest);

        await _botService.ProcessAsync(_httpRequestMock.Object);

        _fallbackMock.Verify(function => function(It.IsAny<TelegramRequest>()), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_CallsLogger_WhenCommandServiceThrows()
    {
        var request = new TelegramRequest(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessageAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception());
        _loggerMock
            .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.HttpRequestToTelegramUpdateAsync(It.IsAny<HttpRequest>()))
            .ReturnsAsync(_updatePayload);
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.UpdateToTelegramRequest(It.IsAny<Update>()))
            .Returns(_telegramRequest);

        await _botService.ProcessAsync(_httpRequestMock.Object);

        _loggerMock
            .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task ProcessAsync_CallsLogger_WhenFunctionThrows()
    {
        var request = new TelegramRequest(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessageAsync(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start, _functionMock.Object, _fallbackMock.Object));
        _functionMock
            .Setup(function => function(It.IsAny<TelegramRequest>()))
            .ThrowsAsync(new Exception());
        _loggerMock
            .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.HttpRequestToTelegramUpdateAsync(It.IsAny<HttpRequest>()))
            .ReturnsAsync(_updatePayload);
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.UpdateToTelegramRequest(It.IsAny<Update>()))
            .Returns(_telegramRequest);

        await _botService.ProcessAsync(_httpRequestMock.Object);

        _loggerMock
            .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task Process_Async_CallsLogger_WhenFallbackThrows()
    {
        var request = new TelegramRequest(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessageAsync(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start, _functionMock.Object, _fallbackMock.Object));
        _functionMock
            .Setup(function => function(It.IsAny<TelegramRequest>()))
            .ThrowsAsync(new Exception());
        _loggerMock
            .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.HttpRequestToTelegramUpdateAsync(It.IsAny<HttpRequest>()))
            .ReturnsAsync(_updatePayload);
        _modelConverterServiceMock
            .Setup(modelConverterService => modelConverterService.UpdateToTelegramRequest(It.IsAny<Update>()))
            .Returns(_telegramRequest);

        await _botService.ProcessAsync(_httpRequestMock.Object);

        _loggerMock
            .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    private static Update GenerateUpdatePayload() => new()
    {
        Id = 121212122,
        Message = new Message
        {
            MessageId = 2123,
            From = new User
            {
                Id = 552374586,
                IsBot = false,
                FirstName = "firstname",
                Username = "username",
                LanguageCode = "en"
            },
            Chat = new Chat
            {
                Id = 552374586,
                FirstName = "firstname",
                Username = "username",
                Type = ChatType.Private
            },
            Date = new DateTime(3254851313),
            Text = "/start",
            Entities = new List<MessageEntity>
            {
                new MessageEntity
                {
                    Offset = 0,
                    Length = 6,
                    Type = MessageEntityType.BotCommand
                }
            }.ToArray()
        }
    };
}
