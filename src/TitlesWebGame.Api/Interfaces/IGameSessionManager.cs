using TitlesWebGame.Api.Models;

namespace TitlesWebGame.Api.Services
{
    public interface IGameSessionManager
    {
        string CreateSession(GameSessionPlayer ownerSessionPlayer);
        bool DeleteSession(string roomKey);
        bool JoinSession(string roomKey, GameSessionPlayer gameSessionPlayer);
        void StartSession(string roomKey, string connectionId);
        bool AddAnswer(string roomKey, GameRoundAnswer gameRoundAnswer);
    }
}