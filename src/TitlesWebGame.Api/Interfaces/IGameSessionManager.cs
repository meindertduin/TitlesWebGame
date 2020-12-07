using System.Collections.Generic;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.Api.Services
{
    public interface IGameSessionManager
    {
        string CreateSession(GameSessionPlayer ownerSessionPlayer);
        bool DeleteSession(string roomKey);
        List<GameSessionPlayer> JoinSession(string roomKey, GameSessionPlayer gameSessionPlayer)
        void StartSession(string roomKey, string connectionId);
        bool AddAnswer(string roomKey, GameRoundAnswer gameRoundAnswer);
    }
}