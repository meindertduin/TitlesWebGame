using System.Threading.Tasks;
using TitlesWebGame.Api.Models;

namespace TitlesWebGame.Api.Services
{
    public interface IGameSessionControllerService
    {
        Task PlaySessionGame(GameSessionState gameSessionState, int titleRounds, int roundsPerTitle);
    }
}