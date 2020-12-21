using System.Collections.Generic;
using System.Threading.Tasks;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Models
{
    public interface IGameRound
    {
        bool AddAnswer(GameRoundAnswer answer);
        Task PlayRound();
        List<(string, int)> StopRound();
        List<GameRoundAnswer> GetRoundAnswers();
    }
}