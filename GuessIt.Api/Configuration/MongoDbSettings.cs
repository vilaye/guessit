namespace GuessIt.Api.Configuration;

public class MongoDbSettings
{
    public const string SectionName = "MongoDB";
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}
