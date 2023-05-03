﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RentBot.Commands.Interfaces;
using RentBot.Factories;
using RentBot.Model;

[assembly: InternalsVisibleTo("RentBot.Tests")]
namespace RentBot.Commands
{
    public class LinkedCommand : ILinkedCommand
    {
        public string CommandMessage { get; }
        public List<ILinkedCommand> ChildCommands { get; set; }
        public Func<IClientFactory, TelegramRequest, Task> Function { get; }
        public Func<IClientFactory, TelegramRequest, Task> Fallback { get; }

        public LinkedCommand(string commandMessage,
            Func<IClientFactory, TelegramRequest, Task> function = default,
            Func<IClientFactory, TelegramRequest, Task> fallback = default)
        {
            CommandMessage = commandMessage;
            ChildCommands = new List<ILinkedCommand>();
            Function = function != default ? function : (clientFactory, request) => default;
            Fallback = fallback != default ? fallback : (clientFactory, request) => default;
        }
    }
}
