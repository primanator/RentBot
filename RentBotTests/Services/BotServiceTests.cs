//using System.Threading.Tasks;
//using Moq;
//using NUnit.Framework;
//using RentBot.Factories;
//using RentBot.Model;
//using RentBot.Services.Interfaces;
//using RentBot.Services.Implementation;
//using Microsoft.Extensions.Logging;
//using Telegram.Bot.Types;
//using RentBot.Commands;
//using RentBot.Constants;
//using System;
//using Telegram.Bot.Types.Enums;
//using System.Collections.Generic;

//namespace RentBot.Tests.Services
//{
//    [TestFixture]
//    public class BotServiceTests
//    {
//        private Mock<IClientFactory> _clientFactoryMock;
//        private Mock<ICommandService> _commandServiceMock;
//        private Mock<ILogger> _loggerMock;
//        private Mock<Func<IClientFactory, TelegramRequest, Task>> _functionMock;
//        private BotService _botService;
//        private Update _updatePayload;

//        [SetUp]
//        public void SetUp()
//        {
//            _clientFactoryMock = new Mock<IClientFactory>();
//            _commandServiceMock = new Mock<ICommandService>();
//            _loggerMock = new Mock<ILogger>();
//            _functionMock = new Mock<Func<IClientFactory, TelegramRequest, Task>>();
//            _botService = new BotService(_clientFactoryMock.Object, _commandServiceMock.Object, _loggerMock.Object);
//            _updatePayload = GenerateUpdatePayload();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _clientFactoryMock = null;
//            _commandServiceMock = null;
//            _loggerMock = null;
//            _functionMock = null;
//            _botService = null;
//            _updatePayload = null;
//        }

//        [Test]
//        public async Task ProcessAsync_WhenCalled_ShouldCallCommandServiceGetCommandByMessage()
//        {
//            var request = new TelegramRequest(_updatePayload);
//            _commandServiceMock
//                .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
//                .Returns(new LinkedCommand(Messages.Start));

//            await _botService.ProcessAsync(request);
            
//            _commandServiceMock.Verify(commandService => commandService.GetCommandByMessage(It.IsAny<string>()), Times.Once);
//        }

//        [Test]
//        public async Task ProcessAsync_WhenCalled_ShouldCallCommandServiceGetCommandByMessageWithCorrectMessage()
//        {
//            var request = new TelegramRequest(_updatePayload);
//            _commandServiceMock
//                .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
//                .Returns(new LinkedCommand(Messages.Start));

//            await _botService.ProcessAsync(request);
            
//            _commandServiceMock.Verify(commandService => commandService.GetCommandByMessage(Messages.Start), Times.Once);
//        }

//        [Test]
//        public async Task ProcessAsync_WhenCalled_ShouldCallFunctionReturnedFromGetCommandMessageOfCommandService()
//        {
//            var request = new TelegramRequest(_updatePayload);
//            var command = new LinkedCommand(Messages.Start, _functionMock.Object);
//            _commandServiceMock
//                .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
//                .Returns(command);

//            await _botService.ProcessAsync(request);

//            _functionMock.Verify(function => function.Invoke(It.IsAny<IClientFactory>(), It.IsAny<TelegramRequest>()), Times.Once);
//        }

//        [Test]
//        public async Task ProcessAsync_WhenCalled_ShouldCallFallbackReturnedFromGetCommandMessageOfCommandService()
//        {
//            var request = new TelegramRequest(_updatePayload);
//            var command = new LinkedCommand(Messages.Start, _functionMock.Object, _functionMock.Object);
//            _commandServiceMock
//                .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
//                .Returns(command);

//            await _botService.ProcessAsync(request);

//            _functionMock.Verify(function => function.Invoke(It.IsAny<IClientFactory>(), It.IsAny<TelegramRequest>()), Times.Exactly(2));
//        }

//        [Test]
//        public async Task ProcessAsync_WhenCommandServiceThrows_ShouldCallLogger()
//        {
//            var request = new TelegramRequest(_updatePayload);
//            _commandServiceMock
//                .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
//                .Throws(new Exception());
//            _loggerMock
//                .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

//            await _botService.ProcessAsync(request);

//            _loggerMock
//                .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
//        }

//        [Test]
//        public async Task ProcessAsync_WhenFunctionThrows_ShouldCallLogger()
//        {
//            var request = new TelegramRequest(_updatePayload);
//            var command = new LinkedCommand(Messages.Start, _functionMock.Object);
//            _commandServiceMock
//                .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
//                .Returns(command);
//            _functionMock
//                .Setup(function => function.Invoke(It.IsAny<IClientFactory>(), It.IsAny<TelegramRequest>()))
//                .Throws(new Exception());
//            _loggerMock
//                .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

//            await _botService.ProcessAsync(request);

//            _loggerMock
//                .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
//        }

//        [Test]
//        public async Task ProcessAsync_WhenFallbackThrows_ShouldCallLogger()
//        {
//            var request = new TelegramRequest(_updatePayload);
//            var command = new LinkedCommand(Messages.Start, _functionMock.Object, _functionMock.Object);
//            _commandServiceMock
//                .Setup(commandService => commandService.GetCommandByMessage(It.IsAny<string>()))
//                .Returns(command);
//            _functionMock
//                .Setup(function => function.Invoke(It.IsAny<IClientFactory>(), It.IsAny<TelegramRequest>()))
//                .Throws(new Exception());
//            _loggerMock
//                .Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

//            await _botService.ProcessAsync(request);

//            _loggerMock
//                .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
//        }

//        private Update GenerateUpdatePayload() => new Update
//        {
//            Id = 121212122,
//            Message = new Message
//            {
//                MessageId = 2123,
//                From = new User
//                {
//                    Id = 552374586,
//                    IsBot = false,
//                    FirstName = "firstname",
//                    Username = "username",
//                    LanguageCode = "en"
//                },
//                Chat = new Chat
//                {
//                    Id = 552374586,
//                    FirstName = "firstname",
//                    Username = "username",
//                    Type = ChatType.Private
//                },
//                Date = new DateTime(3254851313),
//                Text = "/start",
//                Entities = new List<MessageEntity>
//                    {
//                        new MessageEntity
//                        {
//                            Offset = 0,
//                            Length = 6,
//                            Type = MessageEntityType.BotCommand
//                        }
//                    }.ToArray()
//            }
//        };
//    }
//}
