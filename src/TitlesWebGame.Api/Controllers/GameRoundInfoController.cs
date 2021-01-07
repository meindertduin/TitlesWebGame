using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TitlesWebGame.Api.Infrastructure.Repositories;
using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Api.Controllers
{
    [ApiController]
    [Route("/api/gameroundinfo")]
    public class GameRoundInfoController : ControllerBase
    {
        private readonly IGameRoundInfoRepository _gameRoundInfoRepository;

        public GameRoundInfoController(IGameRoundInfoRepository gameRoundInfoRepository)
        {
            _gameRoundInfoRepository = gameRoundInfoRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetRandom()
        {
            var result = await _gameRoundInfoRepository.GetRandomRounds(
                new[] {(int) GameRoundsType.MultipleChoiceRound, (int) GameRoundsType.CompetitiveArtistRound}, 1);
            return Ok(result);
        }
    }
}