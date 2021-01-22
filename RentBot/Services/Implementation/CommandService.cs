using System.Collections.Generic;
using System.Linq;
using RentBot.Commands;
using RentBot.Services.Interfaces;
using Telegram.Bot;

namespace RentBot.Services.Implementation
{
    internal class CommandService : ICommandService
    {
        private readonly List<AbstractCommand> _commands;

        public CommandService(ITelegramBotClient botClient)
        {
            _commands = new List<AbstractCommand>
            {
                new StartCommand(botClient),
                new PathCommand(botClient),
                new PlacesCommand(botClient)
            };
        }

        public bool TryGetCommandForMessage(string message, out ICommand command)
        {
            command = _commands.FirstOrDefault(cmd => cmd.Name == message);
            return command != null;
        }

        public ICommand GetDefaultCommand()
        {
            return _commands.FirstOrDefault(cmd => cmd.Name == ListOfCommands.Start);
        }
    }
}
