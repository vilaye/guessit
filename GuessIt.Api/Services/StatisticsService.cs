using GuessIt.Shared.Models;
using MongoDB.Driver;

namespace GuessIt.Api.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IMongoCollection<GameSession> _sessions;
    private readonly IMongoCollection<GuessAttempt> _attempts;

    public StatisticsService(IMongoDatabase database)
    {
        _sessions = database.GetCollection<GameSession>("sessions");
        _attempts = database.GetCollection<GuessAttempt>("attempts");

        var indexKeys = Builders<GuessAttempt>.IndexKeys.Ascending(a => a.SessionId);
        _attempts.Indexes.CreateOne(new CreateIndexModel<GuessAttempt>(indexKeys));
    }

    public void AddGuessAttempt(GuessAttempt attempt)
    {
        _attempts.InsertOne(attempt);
    }

    public void AddGameSession(GameSession session)
    {
        _sessions.InsertOne(session);
    }

    public IReadOnlyList<GameSession> GetAllSessions()
    {
        return _sessions
            .Find(_ => true)
            .SortByDescending(s => s.CompletedAt)
            .ToList();
    }

    public IReadOnlyList<GuessAttempt> GetAttemptsBySession(Guid sessionId)
    {
        return _attempts
            .Find(a => a.SessionId == sessionId)
            .SortBy(a => a.AttemptNumber)
            .ToList();
    }
}
