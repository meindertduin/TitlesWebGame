using Microsoft.AspNetCore.Mvc;
using TitlesWebGame.Api.Services;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Controllers
{
    [ApiController]
    [Route("api/titlesGame")]
    public class TitlesGameController : ControllerBase
    {
        private readonly IGameSessionManager _gameSessionManager;

        public TitlesGameController(IGameSessionManager gameSessionManager)
        {
            _gameSessionManager = gameSessionManager;
        }
        

        [HttpPost("dataAnswer/{roomKey}")]
        public IActionResult UploadAnswerWithData([FromRoute] string roomKey,[FromBody] GameRoundAnswer answer)
        {
            _gameSessionManager.AddAnswer(roomKey, answer);
            return Ok();
        }

        [HttpGet("dataAnswer/{roomKey}")]
        public IActionResult GetGameRoundAnswers([FromRoute] string roomKey, [FromQuery] string[] connections)
        {
            var answers = _gameSessionManager.GetGameRoundAnswers(roomKey, connections);
            return Ok(answers);
        }

    }
}