using RentBot.Commands.Interfaces;

namespace RentBot.Services.Interfaces
{
    public interface ICommandService
    {
        bool TryGetCommandForMessage(string message, out ICommand command);
        ICommand GetDefaultCommand();
    }
}