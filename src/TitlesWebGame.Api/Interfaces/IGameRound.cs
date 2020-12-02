using System.Collections.Generic;
using System.Threading.Tasks;

namespace TitlesWebGame.Api.Models
{
    public interface IGameRound
    {
        Task<List<(string, int)>> PlayRound();
    }
}