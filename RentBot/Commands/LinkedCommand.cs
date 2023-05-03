using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RentBot.Commands.Interfaces;
using RentBot.Model;

[assembly: InternalsVisibleTo("RentBot.Tests")]
namespace RentBot.Commands;

public class LinkedCommand : ILinkedCommand
{
    public string CommandMessage { get; }
    public List<ILinkedCommand> ChildCommands { get; set; }
    public Func<Request, Task> Function { get; }
    public Func<Request, Task> Fallback { get; }

    public LinkedCommand(string commandMessage,
        Func<Request, Task> function = default,
        Func<Request, Task> fallback = default)
    {
        CommandMessage = commandMessage;
        ChildCommands = new List<ILinkedCommand>();
        Function = function != default ? function : (request) => default;
        Fallback = fallback != default ? fallback : (request) => default;
    }
}
