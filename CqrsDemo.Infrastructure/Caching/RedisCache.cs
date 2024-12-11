using Polly;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CqrsDemo.Infrastructure.Caching
{
    public class RedisCache
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly AsyncPolicy _retryPolicy;

        public RedisCache()
        {
            var connectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _retryPolicy = Policy
                .Handle<RedisException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"🔄 Redis Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
                    });
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var db = _redis.GetDatabase();
                var serializedValue = JsonSerializer.Serialize(value);
                await db.StringSetAsync(key, serializedValue);
            });
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var db = _redis.GetDatabase();
                var serializedValue = await db.StringGetAsync(key);
                return string.IsNullOrEmpty(serializedValue) ? default : JsonSerializer.Deserialize<T>(serializedValue);
            });
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var db = _redis.GetDatabase();
                return await db.KeyDeleteAsync(key);
            });
        }


        public async Task<IEnumerable<T>> GetAllAsync<T>(string pattern = "*")
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var db = _redis.GetDatabase();
                var server = _redis.GetServer(_redis.GetEndPoints().First()); // Get the first available endpoint
                var keys = server.Keys(pattern: pattern);

                var result = new List<T>();

                foreach (var key in keys)
                {
                    var serializedValue = await db.StringGetAsync(key);
                    if (!string.IsNullOrEmpty(serializedValue))
                    {
                        var value = JsonSerializer.Deserialize<T>(serializedValue);
                        if (value != null)
                        {
                            result.Add(value);
                        }
                    }
                }


                return result;
            });
        }

        public async Task<IEnumerable<string>> GetAllKeysAsync(string pattern = "*")
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern);
                return await Task.FromResult(keys.Select(k => k.ToString()).ToList());
            });
        }

        public async Task DeleteBatchAsync(IEnumerable<string> keys)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var db = _redis.GetDatabase();
                var tasks = keys.Select(key => db.KeyDeleteAsync(key));
                return await Task.WhenAll(tasks);
            });
        }
    }
}
