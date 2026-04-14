using ApiClient.Domain;
using Aws.Infra.Interfaces;
using Colaboracao.Helper;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aws.Infra.Impl
{
    public class AwsCacheServiceImpl : IAwsCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _cache;

        public AwsCacheServiceImpl(IConfiguration configuration)
        {
            var options = new ConfigurationOptions
            {
                EndPoints = { VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AWS_CACHE_URL) },
                Ssl = true,
                AbortOnConnectFail = false,
                SyncTimeout = 60000,
                AsyncTimeout = 60000,
                ConnectTimeout = 60000
            };
            try
            {
                _redis = ConnectionMultiplexer.Connect(options);
                _cache = _redis.GetDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao conectar ao cache AWS: " + ex.Message);
            }
        }
        public async Task<T> GetAsync<T>(string key) where T : class
        {
            try
            {
                var value = await _cache.StringGetAsync(key);
                if (!value.HasValue)
                    return null;

                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                // TODO: Adicionar log do erro
                return null;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expirationTime = null) where T : class
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                return await _cache.StringSetAsync(key, serializedValue, expirationTime);
            }
            catch (Exception ex)
            {
                // TODO: Adicionar log do erro
                return false;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                return await _cache.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                // TODO: Adicionar log do erro
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _cache.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                // TODO: Adicionar log do erro
                return false;
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expirationTime = null) where T : class
        {
            try
            {
                var value = await GetAsync<T>(key);
                if (value != null)
                    return value;

                value = await factory();
                if (value != null)
                    await SetAsync(key, value, expirationTime);

                return value;
            }
            catch (Exception ex)
            {
                // TODO: Adicionar log do erro
                return null;
            }
        }

        public async Task<bool> UpdateExpirationAsync(string key, TimeSpan expirationTime)
        {
            try
            {
                return await _cache.KeyExpireAsync(key, expirationTime);
            }
            catch (Exception ex)
            {
                // TODO: Adicionar log do erro
                return false;
            }
        }
    }
}