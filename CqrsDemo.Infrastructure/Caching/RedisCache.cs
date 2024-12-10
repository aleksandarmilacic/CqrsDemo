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
            _redis = ConnectionMultiplexer.Connect("localhost");
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
    }
}
