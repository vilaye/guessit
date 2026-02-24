using GuessIt.Shared.Models;

namespace GuessIt.App.Services;

public class GameService
{
    private readonly Random _random = new();
    private int _targetNumber;
    private int _rangeMin;
    private int _rangeMax;

    public int RangeMin => _rangeMin;
    public int RangeMax => _rangeMax;
    public int TargetNumber => _targetNumber;

    public void StartNewGame(int rangeMin, int rangeMax)
    {
        _rangeMin = rangeMin;
        _rangeMax = rangeMax;
        _targetNumber = _random.Next(rangeMin, rangeMax + 1);
    }

    public GuessResult EvaluateGuess(int guess)
    {
        if (guess < _targetNumber)
            return GuessResult.TooLow;

        if (guess > _targetNumber)
            return GuessResult.TooHigh;

        return GuessResult.Correct;
    }
}
