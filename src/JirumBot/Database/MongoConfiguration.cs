namespace JirumBot.Database;

public sealed record MongoConfiguration
{
    public string ConnectionString { get; init; }
    public string Database { get; init; }
}