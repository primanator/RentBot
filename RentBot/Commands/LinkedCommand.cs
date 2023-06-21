using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RentBot.Commands.Interfaces;
using RentBot.Models;

namespace RentBot.Commands;

public class LinkedCommand : ILinkedCommand
{
    public string CommandMessage { get; }
    public List<ILinkedCommand> ChildCommands { get; set; }
    public Func<TelegramRequest, Task> Function { get; }
    public Func<TelegramRequest, Task> Fallback { get; }

    public LinkedCommand(string commandMessage,
        Func<TelegramRequest, Task> function = default,
        Func<TelegramRequest, Task> fallback = default)
    {
        CommandMessage = commandMessage;
        ChildCommands = new List<ILinkedCommand>();
        Function = function != default ? function : (request) => default;
        Fallback = fallback != default ? fallback : (request) => default;
    }
}
