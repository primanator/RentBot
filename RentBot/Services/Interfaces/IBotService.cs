using System.Threading.Tasks;
using RentBot.Model;

namespace RentBot.Services.Interfaces;

public interface IBotService
{
    Task ProcessAsync(Request request);
}