namespace GuessIt.Api.Services;

public class DashboardService(IStatisticsService statisticsService) : IDashboardService
{
    public DashboardData GetDashboardData()
    {
        var allSessions = statisticsService.GetAllSessions();
        var wonSessions = allSessions.Where(s => s.Won).ToList();

        var data = new DashboardData
        {
            TotalGames = allSessions.Count,
            AverageAttempts = wonSessions.Count > 0
                ? Math.Round(wonSessions.Average(s => s.TotalAttempts), 1) : 0,
            AverageDuration = wonSessions.Count > 0
                ? TimeSpan.FromTicks((long)wonSessions.Average(s => s.Duration.Ticks))
                : TimeSpan.Zero,
            FastestWin = wonSessions.Count > 0
                ? wonSessions.Min(s => s.Duration)
                : TimeSpan.Zero,
            LeastAttempts = wonSessions.Count > 0
                ? wonSessions.Min(s => s.TotalAttempts) : 0
        };

        // Statistiken pro Schwierigkeit
        var difficulties = new[] { (10, "Leicht"), (50, "Mittel"), (100, "Schwer") };
        data.DifficultyStats = difficulties.Select(d =>
        {
            var sessions = wonSessions.Where(s => s.RangeMax == d.Item1).ToList();
            return new DifficultyStats
            {
                Label = d.Item2,
                AverageAttempts = sessions.Count > 0
                    ? Math.Round(sessions.Average(s => s.TotalAttempts), 1) : 0,
                AverageSeconds = sessions.Count > 0
                    ? Math.Round(sessions.Average(s => s.Duration.TotalSeconds), 1) : 0,
                GameCount = sessions.Count
            };
        }).ToList();

        // Bestenliste
        data.Leaderboard = wonSessions
            .OrderBy(s => s.Duration)
            .Take(20)
            .Select((s, i) => new LeaderboardEntry
            {
                Rank = i + 1,
                Difficulty = s.RangeMax switch
                {
                    10 => "Leicht",
                    50 => "Mittel",
                    100 => "Schwer",
                    _ => $"1-{s.RangeMax}"
                },
                Attempts = s.TotalAttempts,
                Duration = s.Duration,
                DurationFormatted = s.Duration.ToString(@"mm\:ss"),
                CompletedAt = s.CompletedAt
            })
            .ToList();

        return data;
    }
}
