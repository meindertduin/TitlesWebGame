using System.Threading.Tasks;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public interface IGameSessionControllerService
    {
        Task PlaySessionGame(GameSessionState gameSessionState, GameSessionStartOptions gameSessionStartOptions);
    }
}