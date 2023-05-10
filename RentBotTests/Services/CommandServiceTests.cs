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
using Telegram.Bot;
using RentBot.Clients.Interfaces;

namespace RentBot.Tests.Services;

[TestFixture]
public class CommandServiceTests
{
    private Mock<ITelegramBotClient> _telegramBotClientMock;
    private Mock<IBlobServiceClientWrapper> _blobServiceClientMock;

    [SetUp]
    public void SetUp()
    {
        _telegramBotClientMock = new Mock<ITelegramBotClient>();
        _blobServiceClientMock = new Mock<IBlobServiceClientWrapper>();
    }

    [TearDown]
    public void TearDown()
    {
        _telegramBotClientMock = null;
        _blobServiceClientMock = null;
    }

    [TestCase(Messages.Start)]
    [TestCase(Messages.FallBack)]
    [TestCase(Messages.Default)]
    [TestCase(Messages.Path)]
    [TestCase(Messages.Places)]
    [TestCase(Messages.Home)]
    [TestCase(Messages.Field)]
    [TestCase(Messages.River)]
    [TestCase(Messages.Forest)]
    [TestCase(Messages.BusSchedule)]
    [TestCase(Messages.HomeGeo)]
    [TestCase(Messages.ToCity)]
    [TestCase(Messages.FromCity)]
    [TestCase(Messages.FAQ)]
    [TestCase(Messages.About)]
    [TestCase(Messages.Minimarket)]
    [TestCase(Messages.Supermarket)]
    [TestCase(Messages.Takeout)]
    [TestCase(Messages.MinimarketLocation)]
    [TestCase(Messages.SupermarketLocation)]
    [TestCase(Messages.TakeoutLocation)]
    [TestCase(Messages.Food)]
    public async Task GetCommandByMessage_ReturnsCommandForMessage_ByDefault(string message)
    {
        var commandService = new CommandService(_telegramBotClientMock.Object, _blobServiceClientMock.Object);
        
        var result = await commandService.GetCommandByMessage(message);

        Assert.That(result.CommandMessage, Is.EqualTo(message));
    }
}
