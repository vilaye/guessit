namespace GuessIt.Api.Services;

public interface IDashboardService
{
    DashboardData GetDashboardData();
}

public class DashboardData
{
    public int TotalGames { get; set; }
    public double AverageAttempts { get; set; }
    public TimeSpan AverageDuration { get; set; }
    public TimeSpan FastestWin { get; set; }
    public int LeastAttempts { get; set; }
    public List<DifficultyStats> DifficultyStats { get; set; } = [];
    public List<LeaderboardEntry> Leaderboard { get; set; } = [];
}

public class DifficultyStats
{
    public string Label { get; set; } = "";
    public double AverageAttempts { get; set; }
    public double AverageSeconds { get; set; }
    public int GameCount { get; set; }
}

public class LeaderboardEntry
{
    public int Rank { get; set; }
    public string Difficulty { get; set; } = "";
    public int Attempts { get; set; }
    public TimeSpan Duration { get; set; }
    public string DurationFormatted { get; set; } = "";
    public DateTime CompletedAt { get; set; }
}
