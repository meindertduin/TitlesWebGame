using System.Threading.Tasks;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.Api.Services
{
    public interface IGameRoundController
    {
        Task PlayGameRound(GameSessionState gameSessionState, GameRoundInfo gameRoundInfo);
    }
}