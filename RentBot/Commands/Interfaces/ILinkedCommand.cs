using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RentBot.Model;

[assembly: InternalsVisibleTo("RentBot.Tests")]
namespace RentBot.Commands.Interfaces;

public interface ILinkedCommand
{
    string CommandMessage { get; }
    List<ILinkedCommand> ChildCommands { get; }
    Func<Request, Task> Function { get; }
    Func<Request, Task> Fallback { get; }
}
