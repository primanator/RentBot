using RentBot.Commands;

namespace RentBot.Services.Interfaces
{
    public interface ICommandService
    {   
        LinkedCommand GetCommandByMessage(string message);
    }
}
