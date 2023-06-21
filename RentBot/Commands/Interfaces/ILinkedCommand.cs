using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RentBot.Models;

namespace RentBot.Commands.Interfaces;

public interface ILinkedCommand
{
    string CommandMessage { get; }
    List<ILinkedCommand> ChildCommands { get; }
    Func<TelegramRequest, Task> Function { get; }
    Func<TelegramRequest, Task> Fallback { get; }
}
