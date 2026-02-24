using System.Collections.Concurrent;
using GuessIt.Shared.Models;

namespace GuessIt.Api.Services;

public class StatisticsService : IStatisticsService
{
    private readonly ConcurrentBag<GuessAttempt> _attempts = [];
    private readonly ConcurrentBag<GameSession> _sessions = [];

    public void AddGuessAttempt(GuessAttempt attempt)
    {
        _attempts.Add(attempt);
    }

    public void AddGameSession(GameSession session)
    {
        _sessions.Add(session);
    }

    public IReadOnlyList<GameSession> GetAllSessions()
    {
        return _sessions
            .OrderByDescending(s => s.CompletedAt)
            .ToList();
    }

    public IReadOnlyList<GuessAttempt> GetAttemptsBySession(Guid sessionId)
    {
        return _attempts
            .Where(a => a.SessionId == sessionId)
            .OrderBy(a => a.AttemptNumber)
            .ToList();
    }
}
