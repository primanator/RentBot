using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RentBot.Factories;
using RentBot.Model;

namespace RentBot.Commands.Interfaces
{
    public interface ILinkedCommand
    {
        string CommandMessage { get; }
        List<LinkedCommand> ChildCommands { get; }
        Func<IClientFactory, TelegramRequest, Task> Function { get; }
        Func<IClientFactory, TelegramRequest, Task> Fallback { get; }
    }
}
