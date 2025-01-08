
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveyBasket.Api.Services;

public class CasheService(IDistributedCache distributedCache) : ICasheService
{
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cashedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

        if(cashedValue is null)
            return null;

        return JsonSerializer.Deserialize<T>(cashedValue);
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }
}
