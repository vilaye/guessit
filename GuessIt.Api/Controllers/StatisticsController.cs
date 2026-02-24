using Microsoft.AspNetCore.Mvc;
using GuessIt.Api.Services;
using GuessIt.Shared.Models;

namespace GuessIt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
{
    [HttpPost("guess")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult PostGuess([FromBody] GuessAttempt attempt)
    {
        statisticsService.AddGuessAttempt(attempt);
        return Ok();
    }

    [HttpPost("session")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult PostSession([FromBody] GameSession session)
    {
        statisticsService.AddGameSession(session);
        return Ok();
    }

    [HttpGet("sessions")]
    [ProducesResponseType(typeof(IReadOnlyList<GameSession>), StatusCodes.Status200OK)]
    public IActionResult GetSessions()
    {
        return Ok(statisticsService.GetAllSessions());
    }

    [HttpGet("sessions/{sessionId:guid}/attempts")]
    [ProducesResponseType(typeof(IReadOnlyList<GuessAttempt>), StatusCodes.Status200OK)]
    public IActionResult GetAttempts(Guid sessionId)
    {
        return Ok(statisticsService.GetAttemptsBySession(sessionId));
    }
}
