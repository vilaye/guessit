namespace GuessIt.Shared.Models;

public class GuessAttempt
{
    public Guid SessionId { get; set; }
    public int GuessedNumber { get; set; }
    public GuessResult Result { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
