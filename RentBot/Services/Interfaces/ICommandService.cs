using RentBot.Commands.Interfaces;
using System.Threading.Tasks;

namespace RentBot.Services.Interfaces;

public interface ICommandService
{   
    Task<ILinkedCommand> GetCommandByMessage(string message);
}
