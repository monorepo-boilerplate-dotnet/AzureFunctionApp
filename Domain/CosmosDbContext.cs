using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Domain;

public class CosmosDbSettings
{
    public string AccountEndpoint { get; set; } = string.Empty;
    public string AccountKey { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public int? Throughput { get; set; } = 400; // RU/s mặc định
}

public class CosmosDbContext
{
    private readonly CosmosClient _client;
    private readonly Database _database;

    public CosmosDbContext(IOptions<CosmosDbSettings> settings)
    {
        var cfg = settings.Value;
        _client = new CosmosClient(cfg.AccountEndpoint, cfg.AccountKey, new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            },
            AllowBulkExecution = true
        });

        _database = _client.CreateDatabaseIfNotExistsAsync(cfg.DatabaseName, cfg.Throughput).GetAwaiter()
            .GetResult();
    }

    public Container GetContainer(string containerName)
    {
        return _database.GetContainer(containerName);
    }

    public async Task<Container> EnsureContainerAsync(string containerName, string partitionKey)
    {
        var response = await _database.CreateContainerIfNotExistsAsync(
            containerName,
            partitionKey,
            400
        );
        return response.Container;
    }
}