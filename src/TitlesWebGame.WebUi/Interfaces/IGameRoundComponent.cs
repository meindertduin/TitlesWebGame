using System.Threading.Tasks;

namespace TitlesWebGame.WebUi.Interfaces
{
    public interface IGameRoundComponent
    {
        Task HandleTimerDone();
    }
}