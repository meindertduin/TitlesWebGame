using System.Collections.Generic;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public interface IGameSessionManager
    {
        GameSessionInitViewModel CreateSession(GameSessionPlayer ownerSessionPlayer);
        bool DeleteSession(string roomKey);
        GameSessionInitViewModel JoinSession(string roomKey, GameSessionPlayer gameSessionPlayer);
        void StartSession(string roomKey, string connectionId, GameSessionStartOptions gameSessionStartOptions);
        bool AddAnswer(string roomKey, GameRoundAnswer gameRoundAnswer);
        bool PlayAgain(string roomKey, string connectionId);
        List<GameRoundAnswer> GetGameRoundAnswers(string roomKey);
    }
}