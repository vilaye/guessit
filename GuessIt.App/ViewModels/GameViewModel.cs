using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using GuessIt.App.Commands;
using GuessIt.App.Services;
using GuessIt.Shared.Models;

namespace GuessIt.App.ViewModels;

public class GameViewModel : INotifyPropertyChanged
{
    private readonly GameService _gameService = new();
    private readonly StatisticsApiClient _apiClient = new();
    private readonly DispatcherTimer _timer;

    private Guid _sessionId;
    private DateTime _startTime;

    private string _guessInput = string.Empty;
    private string _feedbackMessage = "Rate eine Zahl!";
    private string _feedbackColor = "#555555";
    private int _attemptCount;
    private string _elapsedTime = "00:00";
    private bool _isGameOver;
    private int _selectedDifficultyIndex;
    private string _guessHistory = string.Empty;

    public GameViewModel()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => UpdateElapsedTime();

        GuessCommand = new RelayCommand(_ => ExecuteGuess(), _ => CanGuess());
        NewGameCommand = new RelayCommand(_ => StartNewGame());

        Difficulties = [
            new DifficultyOption("Leicht (1–10)", 1, 10),
            new DifficultyOption("Mittel (1–50)", 1, 50),
            new DifficultyOption("Schwer (1–100)", 1, 100)
        ];

        StartNewGame();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand GuessCommand { get; }
    public ICommand NewGameCommand { get; }
    public DifficultyOption[] Difficulties { get; }

    public string GuessInput
    {
        get => _guessInput;
        set { _guessInput = value; OnPropertyChanged(); }
    }

    public string FeedbackMessage
    {
        get => _feedbackMessage;
        set { _feedbackMessage = value; OnPropertyChanged(); }
    }

    public string FeedbackColor
    {
        get => _feedbackColor;
        set { _feedbackColor = value; OnPropertyChanged(); }
    }

    public int AttemptCount
    {
        get => _attemptCount;
        set { _attemptCount = value; OnPropertyChanged(); }
    }

    public string ElapsedTime
    {
        get => _elapsedTime;
        set { _elapsedTime = value; OnPropertyChanged(); }
    }

    public bool IsGameOver
    {
        get => _isGameOver;
        set { _isGameOver = value; OnPropertyChanged(); }
    }

    public int SelectedDifficultyIndex
    {
        get => _selectedDifficultyIndex;
        set
        {
            if (_selectedDifficultyIndex != value)
            {
                _selectedDifficultyIndex = value;
                OnPropertyChanged();
                StartNewGame();
            }
        }
    }

    public string GuessHistory
    {
        get => _guessHistory;
        set { _guessHistory = value; OnPropertyChanged(); }
    }

    private DifficultyOption CurrentDifficulty => Difficulties[_selectedDifficultyIndex];

    private bool CanGuess() => !IsGameOver && !string.IsNullOrWhiteSpace(GuessInput);

    private void ExecuteGuess()
    {
        if (!int.TryParse(GuessInput, out var guess))
        {
            FeedbackMessage = "Bitte gib eine gültige Zahl ein.";
            FeedbackColor = "#E74C3C";
            return;
        }

        var difficulty = CurrentDifficulty;
        if (guess < difficulty.Min || guess > difficulty.Max)
        {
            FeedbackMessage = $"Bitte gib eine Zahl zwischen {difficulty.Min} und {difficulty.Max} ein.";
            FeedbackColor = "#E74C3C";
            return;
        }

        AttemptCount++;
        var result = _gameService.EvaluateGuess(guess);

        var historyEntry = $"Versuch {AttemptCount}: {guess}";

        switch (result)
        {
            case GuessResult.TooLow:
                FeedbackMessage = $"{guess} ist zu niedrig!";
                FeedbackColor = "#3498DB";
                historyEntry += " ↑ zu niedrig";
                break;
            case GuessResult.TooHigh:
                FeedbackMessage = $"{guess} ist zu hoch!";
                FeedbackColor = "#E67E22";
                historyEntry += " ↓ zu hoch";
                break;
            case GuessResult.Correct:
                var elapsed = DateTime.UtcNow - _startTime;
                FeedbackMessage = $"Richtig! Die Zahl war {guess}. Du hast {AttemptCount} Versuche benötigt ({elapsed:mm\\:ss}).";
                FeedbackColor = "#27AE60";
                historyEntry += " ✓ richtig!";
                IsGameOver = true;
                _timer.Stop();
                SendSessionToApiAsync(elapsed);
                break;
        }

        GuessHistory = string.IsNullOrEmpty(GuessHistory)
            ? historyEntry
            : historyEntry + Environment.NewLine + GuessHistory;

        SendGuessToApiAsync(guess, result);
        GuessInput = string.Empty;
    }

    private void StartNewGame()
    {
        var difficulty = CurrentDifficulty;

        _sessionId = Guid.NewGuid();
        _startTime = DateTime.UtcNow;

        _gameService.StartNewGame(difficulty.Min, difficulty.Max);

        AttemptCount = 0;
        ElapsedTime = "00:00";
        GuessInput = string.Empty;
        GuessHistory = string.Empty;
        FeedbackMessage = $"Rate eine Zahl zwischen {difficulty.Min} und {difficulty.Max}!";
        FeedbackColor = "#555555";
        IsGameOver = false;

        _timer.Start();
    }

    private void UpdateElapsedTime()
    {
        var elapsed = DateTime.UtcNow - _startTime;
        ElapsedTime = elapsed.ToString(@"mm\:ss");
    }

    private async void SendGuessToApiAsync(int guess, GuessResult result)
    {
        await _apiClient.SendGuessAsync(new GuessAttempt
        {
            SessionId = _sessionId,
            GuessedNumber = guess,
            Result = result,
            AttemptNumber = AttemptCount,
            Timestamp = DateTime.UtcNow
        });
    }

    private async void SendSessionToApiAsync(TimeSpan duration)
    {
        var difficulty = CurrentDifficulty;
        await _apiClient.SendSessionAsync(new GameSession
        {
            SessionId = _sessionId,
            RangeMin = difficulty.Min,
            RangeMax = difficulty.Max,
            TargetNumber = _gameService.TargetNumber,
            TotalAttempts = AttemptCount,
            Duration = duration,
            Won = true,
            StartedAt = _startTime,
            CompletedAt = DateTime.UtcNow
        });
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public record DifficultyOption(string Label, int Min, int Max)
{
    public override string ToString() => Label;
}
