using System;
using Colaboracao.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Colaboracao.Core.Impl;

public class MemoryCacheService(IMemoryCache memoryCache) : IMemoryCacheService
{
    public bool GetCachedValue<T>(string key, out T? value)
    {
        if (memoryCache.TryGetValue(key, out var cached) && cached is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    public void SetCachedValue<T>(string key, T? value, TimeSpan expiration = default)
    {
        if (value is null)
            return;

        if (expiration == default)
            expiration = TimeSpan.FromMinutes(2);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        memoryCache.Set(key, value, cacheOptions);
    }
}