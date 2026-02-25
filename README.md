# GuessIt

Ein Zahlenratespiel mit WPF-Desktop-App, REST-API, MongoDB-Persistenz und Statistik-Dashboard.

## Live-Demo

- **Dashboard:** [guessit-api.azurewebsites.net/dashboard](https://guessit-api.azurewebsites.net/dashboard)
- **Swagger API:** [guessit-api.azurewebsites.net/swagger](https://guessit-api.azurewebsites.net/swagger)
- **API Sessions:** [guessit-api.azurewebsites.net/api/statistics/sessions](https://guessit-api.azurewebsites.net/api/statistics/sessions)

## Architektur

```
GuessIt.App      WPF Desktop-App (MVVM)       → Spiel-UI
GuessIt.Api      ASP.NET Core Web API          → Statistik-Endpunkte + Dashboard
GuessIt.Shared   Class Library                 → Gemeinsame Models
```

- **Frontend:** WPF mit MVVM-Pattern (ViewModel, Commands, Converter)
- **Backend:** ASP.NET Core Web API mit Swagger-Dokumentation
- **Dashboard:** Blazor Server mit DevExpress Charts und Grid
- **Datenbank:** MongoDB (mit automatischem Fallback auf In-Memory-Speicher)
- **Hosting:** Azure App Service + MongoDB Atlas

## Schnellstart

### Voraussetzungen

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Nur spielen (einfachste Variante)

Die WPF-App verbindet sich automatisch mit der gehosteten API:

```bash
cd GuessIt.App
dotnet run
```

Fertig. Die Spieldaten werden in der Cloud gespeichert.

### API lokal starten

```bash
cd GuessIt.Api
dotnet run
```

Die API startet mit **In-Memory-Speicher** — alle Features funktionieren, Daten gehen aber beim Neustart verloren. In der Azure-Umgebung wird MongoDB Atlas für persistente Speicherung verwendet.

## Dashboard

Das Statistik-Dashboard ist unter `/dashboard` erreichbar und zeigt:

- **Summary-Cards** — Gesamtzahl Spiele, durchschnittliche Versuche und Zeit, schnellster Sieg
- **Bar-Charts** — Durchschnittliche Versuche und Zeit pro Schwierigkeit (Leicht/Mittel/Schwer)
- **Bestenliste** — Top-Siege sortiert nach schnellster Zeit

Gebaut mit DevExpress Blazor (DxChart, DxGrid) und Blazor Server direkt im API-Projekt.

## API-Endpunkte

| Methode | Endpunkt | Beschreibung |
|---------|----------|--------------|
| `POST`  | `/api/statistics/guess` | Einzelnen Rateversuch speichern |
| `POST`  | `/api/statistics/session` | Abgeschlossene Spielsession speichern |
| `GET`   | `/api/statistics/sessions` | Alle Spielsessions abrufen |
| `GET`   | `/api/statistics/sessions/{id}/attempts` | Rateversuche einer Session |
| `GET`   | `/health` | Health Check |
| `GET`   | `/dashboard` | Statistik-Dashboard |

Swagger-UI: [localhost:5000/swagger](http://localhost:5000/swagger)

## Spielregeln

1. Schwierigkeit wählen: Leicht (1-10), Mittel (1-50), Schwer (1-100)
2. Zahl eingeben und raten
3. Feedback: zu hoch, zu niedrig, oder richtig
4. Ziel: Zahl in möglichst wenigen Versuchen und kurzer Zeit erraten

## Tech-Stack

- .NET 10 / C# 14
- WPF (Windows Presentation Foundation)
- ASP.NET Core Web API
- Blazor Server
- DevExpress Blazor (Charts, Grid)
- MongoDB (mit In-Memory-Fallback)
- Azure App Service
- Swagger / OpenAPI
