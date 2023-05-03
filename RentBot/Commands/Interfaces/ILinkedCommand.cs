using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RentBot.Factories;
using RentBot.Model;

[assembly: InternalsVisibleTo("RentBot.Tests")]
namespace RentBot.Commands.Interfaces
{
    public interface ILinkedCommand
    {
        string CommandMessage { get; }
        List<ILinkedCommand> ChildCommands { get; }
        Func<IClientFactory, TelegramRequest, Task> Function { get; }
        Func<IClientFactory, TelegramRequest, Task> Fallback { get; }
    }
}
