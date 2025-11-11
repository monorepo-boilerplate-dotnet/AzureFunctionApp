using System.Linq.Expressions;
using System.Net;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Infrastructure.Repositories;

public class CosmosRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IClaimsService _claimsService;
    private readonly Container _container;
    private readonly ICurrentTime _timeService;

    public CosmosRepository(Container container, ICurrentTime timeService, IClaimsService claimsService)
    {
        _container = container;
        _timeService = timeService;
        _claimsService = claimsService;
    }

    #region DELETE

    public async Task<bool> HardRemoveAsync(TEntity entity)
    {
        var pk = GetPartitionKey(entity);
        await _container.DeleteItemAsync<TEntity>(entity.Id.ToString(), new PartitionKey(pk));
        return true;
    }

    #endregion

    protected virtual string GetPartitionKey(TEntity entity)
    {
        return entity.Id.ToString();
    }

    #region ADD

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var now = _timeService.GetCurrentTime().ToUniversalTime();
        var userId = _claimsService.CurrentUserId;

        entity.CreatedAt = now;
        entity.UpdatedAt = now;
        entity.CreatedBy = userId;
        entity.UpdatedBy = userId;

        var pk = GetPartitionKey(entity);
        var response = await _container.CreateItemAsync(entity, new PartitionKey(pk));

        entity.ETag = response.ETag; // store for concurrency
        return entity;
    }

    public async Task AddRangeAsync(List<TEntity> entities)
    {
        if (entities.Count == 0) return;

        var userId = _claimsService.CurrentUserId;
        var now = _timeService.GetCurrentTime().ToUniversalTime();
        var pk = GetPartitionKey(entities.First());

        // batch must share same partition key
        var batch = _container.CreateTransactionalBatch(new PartitionKey(pk));

        foreach (var e in entities)
        {
            e.CreatedAt = now;
            e.UpdatedAt = now;
            e.CreatedBy = userId;
            e.UpdatedBy = userId;
            batch.CreateItem(e);
        }

        var result = await batch.ExecuteAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception($"Batch insert failed: {result.StatusCode}");
    }

    #endregion

    #region QUERY

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var queryable = _container.GetItemLinqQueryable<TEntity>()
            .Where(x => !x.IsDeleted);

        if (predicate != null)
            queryable = queryable.Where(predicate);

        var iterator = queryable.ToFeedIterator();
        var results = new List<TEntity>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _container.GetItemLinqQueryable<TEntity>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .ToFeedIterator();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            var item = response.FirstOrDefault();
            if (item != null)
                return item;
        }

        return null;
    }

    public async Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        var data = await GetAllAsync();
        return data.AsQueryable();
    }

    #endregion

    #region UPDATE

    public async Task<bool> Update(TEntity entity)
    {
        var now = _timeService.GetCurrentTime().ToUniversalTime();
        entity.UpdatedAt = now;
        entity.UpdatedBy = _claimsService.CurrentUserId;

        var pk = GetPartitionKey(entity);
        var requestOptions = new ItemRequestOptions
        {
            IfMatchEtag = entity.ETag // concurrency control
        };

        try
        {
            var response =
                await _container.ReplaceItemAsync(entity, entity.Id.ToString(), new PartitionKey(pk), requestOptions);
            entity.ETag = response.ETag;
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
        {
            throw new Exception("Concurrency conflict: the entity was modified by another process.");
        }
    }

    public async Task<bool> UpdateRange(List<TEntity> entities)
    {
        if (entities.Count == 0) return false;

        var pk = GetPartitionKey(entities.First());
        var now = _timeService.GetCurrentTime().ToUniversalTime();
        var userId = _claimsService.CurrentUserId;

        var batch = _container.CreateTransactionalBatch(new PartitionKey(pk));

        foreach (var e in entities)
        {
            e.UpdatedAt = now;
            e.UpdatedBy = userId;
            batch.ReplaceItem(e.Id.ToString(), e);
        }

        var result = await batch.ExecuteAsync();
        if (!result.IsSuccessStatusCode)
            throw new Exception($"Batch update failed: {result.StatusCode}");

        return true;
    }

    #endregion
}