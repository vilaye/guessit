using GuessIt.Api.Configuration;
using GuessIt.Api.Services;
using MongoDB.Driver;

MongoDbConfiguration.Configure();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Blazor Server + DevExpress
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDevExpressBlazor();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// MongoDB mit automatischem Fallback auf In-Memory
var mongoSettings = builder.Configuration
    .GetSection(MongoDbSettings.SectionName)
    .Get<MongoDbSettings>();

if (mongoSettings is not null && !string.IsNullOrEmpty(mongoSettings.ConnectionString))
{
    try
    {
        var client = new MongoClient(mongoSettings.ConnectionString);
        var database = client.GetDatabase(mongoSettings.DatabaseName);

        // Verbindung testen
        database.ListCollectionNames().FirstOrDefault();

        builder.Services.AddSingleton<IMongoClient>(client);
        builder.Services.AddSingleton<IMongoDatabase>(database);
        builder.Services.AddSingleton<IStatisticsService, MongoStatisticsService>();

        Console.WriteLine("MongoDB verbunden — Daten werden persistent gespeichert.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"MongoDB nicht erreichbar ({ex.Message}) — verwende In-Memory-Speicher.");
        builder.Services.AddSingleton<IStatisticsService, InMemoryStatisticsService>();
    }
}
else
{
    Console.WriteLine("Keine MongoDB-Konfiguration gefunden — verwende In-Memory-Speicher.");
    builder.Services.AddSingleton<IStatisticsService, InMemoryStatisticsService>();
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapRazorComponents<GuessIt.Api.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
