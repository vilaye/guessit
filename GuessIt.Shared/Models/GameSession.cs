namespace GuessIt.Shared.Models;

public class GameSession
{
    public Guid SessionId { get; set; }
    public int RangeMin { get; set; }
    public int RangeMax { get; set; }
    public int TargetNumber { get; set; }
    public int TotalAttempts { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Won { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
}
