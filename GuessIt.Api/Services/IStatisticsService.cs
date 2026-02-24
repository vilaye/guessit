using GuessIt.Shared.Models;

namespace GuessIt.Api.Services;

public interface IStatisticsService
{
    void AddGuessAttempt(GuessAttempt attempt);
    void AddGameSession(GameSession session);
    IReadOnlyList<GameSession> GetAllSessions();
    IReadOnlyList<GuessAttempt> GetAttemptsBySession(Guid sessionId);
}
