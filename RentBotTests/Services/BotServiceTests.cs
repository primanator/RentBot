using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RentBot.Services.Interfaces;
using RentBot.Services.Implementation;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using System;
using RentBot.Model;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using RentBot.Commands;
using RentBot.Constants;

namespace RentBot.Tests.Services;

[TestFixture]
public class BotServiceTests
{
    private Mock<ICommandService> _commandServiceMock;
    private Mock<ILogger<BotService>> _loggerMock;
    private Mock<Func<Request, Task>> _functionMock;
    private Mock<Func<Request, Task>> _fallbackMock;
    private BotService _botService;
    private Update _updatePayload;

    [SetUp]
    public void SetUp()
    {
        _commandServiceMock = new Mock<ICommandService>();
        _loggerMock = new Mock<ILogger<BotService>>();
        _functionMock = new Mock<Func<Request, Task>>();
        _fallbackMock = new Mock<Func<Request, Task>>();
        _botService = new BotService(_commandServiceMock.Object, _loggerMock.Object);
        _updatePayload = GenerateUpdatePayload();
    }

    [TearDown]
    public void TearDown()
    {
        _commandServiceMock = null;
        _loggerMock = null;
        _functionMock = null;
        _fallbackMock = null;
        _botService = null;
        _updatePayload = null;
    }

    [Test]
    public async Task ProcessAsync_CallsGetCommandByMessage_ByDefault()
    {
        var request = new Request(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start));

        await _botService.ProcessAsync(request);

        _commandServiceMock.Verify(commnadService => commnadService.GetCommandByMessage(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_CallsCommandFunction_ByDefault()
    {
        var request = new Request(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start, _functionMock.Object, _fallbackMock.Object));

        await _botService.ProcessAsync(request);

        _functionMock.Verify(function => function(It.IsAny<Request>()), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_CallsCommandFallback_ByDefault()
    {
        var request = new Request(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start, _functionMock.Object, _fallbackMock.Object));

        await _botService.ProcessAsync(request);

        _fallbackMock.Verify(function => function(It.IsAny<Request>()), Times.Once);
    }

    [Test]
    public async Task ProcessAsync_CallsLogger_WhenCommandServiceThrows()
    {
        var request = new Request(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
            .ThrowsAsync(new Exception());
        _loggerMock
            .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        await _botService.ProcessAsync(request);

        _loggerMock
            .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task ProcessAsync_CallsLogger_WhenFunctionThrows()
    {
        var request = new Request(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start, _functionMock.Object, _fallbackMock.Object));
        _functionMock
            .Setup(function => function(It.IsAny<Request>()))
            .ThrowsAsync(new Exception());
        _loggerMock
            .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        
        await _botService.ProcessAsync(request);

        _loggerMock
            .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task Process_Async_CallsLogger_WhenFallbackThrows()
    {
        var request = new Request(_updatePayload);
        _commandServiceMock
            .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
            .ReturnsAsync(new LinkedCommand(Messages.Start, _functionMock.Object, _fallbackMock.Object));
        _functionMock
            .Setup(function => function(It.IsAny<Request>()))
            .ThrowsAsync(new Exception());
        _loggerMock
            .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        
        await _botService.ProcessAsync(request);

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
