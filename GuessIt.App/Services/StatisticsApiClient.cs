using System.Net.Http;
using System.Net.Http.Json;
using GuessIt.Shared.Models;

namespace GuessIt.App.Services;

public class StatisticsApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public StatisticsApiClient(string baseUrl = "http://localhost:5000")
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task SendGuessAsync(GuessAttempt attempt)
    {
        try
        {
            await _httpClient.PostAsJsonAsync("api/statistics/guess", attempt);
        }
        catch (HttpRequestException)
        {
            // API ist nicht erreichbar – Spiel läuft trotzdem weiter
        }
    }

    public async Task SendSessionAsync(GameSession session)
    {
        try
        {
            await _httpClient.PostAsJsonAsync("api/statistics/session", session);
        }
        catch (HttpRequestException)
        {
            // API ist nicht erreichbar – Spiel läuft trotzdem weiter
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
