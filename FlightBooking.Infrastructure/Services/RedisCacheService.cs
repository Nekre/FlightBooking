using System.Text.Json;
using FlightBooking.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace FlightBooking.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default(CancellationToken))
    {
        var jsonString = await _distributedCache.GetStringAsync(key, cancellationToken);
        
        if (string.IsNullOrEmpty(jsonString))
            return default;

        return JsonSerializer.Deserialize<T>(jsonString);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var options = new DistributedCacheEntryOptions();
        
        options.AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10);
        var jsonString = JsonSerializer.Serialize(value);
        
        await _distributedCache.SetStringAsync(key, jsonString, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }
}