using Infrastructure.Interfaces;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly IClaimsService _claimsService;
    private readonly ICurrentTime _timeService;

    public UnitOfWork(IClaimsService claimsService, ICurrentTime timeService)
    {
        _claimsService = claimsService;
        _timeService = timeService;
    }

    public void Dispose()
    {
        // DO NOT dispose CosmosDbContext - it's a singleton managed by DI container
        // Disposing it here causes "Cannot access a disposed 'CosmosClient'" errors
        // The DI container will handle its disposal when the application shuts down
    }

    public async Task<int> SaveChangesAsync()
    {
        // Cosmos DB saves automatically when we call CreateItemAsync, UpsertItemAsync, DeleteItemAsync
        // This method is kept for compatibility
        return await Task.FromResult(1);
    }
}