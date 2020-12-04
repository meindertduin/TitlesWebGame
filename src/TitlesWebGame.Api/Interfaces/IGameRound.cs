using System.Collections.Generic;
using System.Threading.Tasks;

namespace TitlesWebGame.Api.Models
{
    public interface IGameRound
    {
        bool AddAnswer(GameRoundAnswer answer);
        Task PlayRound();
        List<(string, int)> StopRound();
    }
}