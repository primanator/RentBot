using RentBot.Commands.Interfaces;

namespace RentBot.Services.Interfaces
{
    public interface ICommandService
    {   
        ILinkedCommand GetCommandByMessage(string message);
    }
}
