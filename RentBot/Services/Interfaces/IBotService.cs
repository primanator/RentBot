using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RentBot.Services.Interfaces;

public interface IBotService
{
    Task ProcessAsync(HttpRequest httpRequest);
}