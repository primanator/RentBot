using RentBot.Commands.Interfaces;
using System.Threading.Tasks;

namespace RentBot.Services.Interfaces;

public interface ICommandService
{   
    Task<ILinkedCommand> GetCommandByMessageAsync(string message);
}
