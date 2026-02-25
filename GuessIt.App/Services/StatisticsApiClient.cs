using System.Net.Http;
using System.Net.Http.Json;
using GuessIt.Shared.Models;

namespace GuessIt.App.Services;

public class StatisticsApiClient : IDisposable
{
    private const string LocalUrl = "http://localhost:5000";
    private const string AzureUrl = "https://guessit-api.azurewebsites.net";

    private readonly HttpClient _httpClient = new();
    private Uri _baseUri = new(LocalUrl);

    public StatisticsApiClient()
    {
        CheckLocalApiAsync();
    }

    private async void CheckLocalApiAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var response = await _httpClient.GetAsync($"{LocalUrl}/health", cts.Token);
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            _baseUri = new Uri(AzureUrl);
        }
    }

    public async Task SendGuessAsync(GuessAttempt attempt)
    {
        try
        {
            await _httpClient.PostAsJsonAsync(new Uri(_baseUri, "api/statistics/guess"), attempt);
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
            await _httpClient.PostAsJsonAsync(new Uri(_baseUri, "api/statistics/session"), session);
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
