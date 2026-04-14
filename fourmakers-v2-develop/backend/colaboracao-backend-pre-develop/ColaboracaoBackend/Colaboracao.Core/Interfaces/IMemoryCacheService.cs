using System;

namespace Colaboracao.Core.Interfaces;

public interface IMemoryCacheService
{
    bool GetCachedValue<T>(string key, out T? value);
    void SetCachedValue<T>(string key, T? value, TimeSpan expiration = default);
}