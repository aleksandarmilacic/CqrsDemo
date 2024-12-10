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

        public RedisCache()
        {
            var connectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";
            _redis = ConnectionMultiplexer.Connect(connectionString);
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var db = _redis.GetDatabase();
            var serializedValue = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, serializedValue);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var serializedValue = await db.StringGetAsync(key);

            return string.IsNullOrEmpty(serializedValue) ? default : JsonSerializer.Deserialize<T>(serializedValue);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string pattern = "*")
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
        }
    }
}
