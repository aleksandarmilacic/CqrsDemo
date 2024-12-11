using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Caching;
using CqrsDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Services
{
    public class ReconciliationService
    {
        private readonly AppDbContext _dbContext;
        private readonly RedisCache _redisCache;

        public ReconciliationService(AppDbContext dbContext, RedisCache redisCache)
        {
            _dbContext = dbContext;
            _redisCache = redisCache;
        }

        /// <summary>
        /// Reconcile the Command Store (SQL) with the Read Store (Redis) for changes since a specific time.
        /// Ensures Redis contains all the latest records from SQL and deletes stale records.
        /// </summary>
        /// <param name="takeSince">The DateTime to sync changes from.</param>
        public async Task ReconcileAsync(DateTime takeSince)
        {
            try
            {
                // Step 1: Get all orders from the SQL command store that changed since `takeSince`
                var commandStoreOrders = await _dbContext.Orders
                    .AsNoTracking()
                    .Where(o => o.CreatedDate >= takeSince || o.ModifiedDate >= takeSince)
                    .ToListAsync();

                var commandStoreOrderIds = commandStoreOrders.Select(o => o.Id.ToString()).ToList();

                // Step 2: Get all keys from the Redis cache that match "order:*"
                var redisKeys = await _redisCache.GetAllKeysAsync("order:*");
                var redisOrderIds = redisKeys.Select(key => key.Replace("order:", "")).ToList();

                // Step 3: Handle **new and updated** records
                var updatedOrders = commandStoreOrders.Where(o => !redisOrderIds.Contains(o.Id.ToString())).ToList();

                if (updatedOrders.Any())
                {
                    var tasks = updatedOrders.Select(order =>
                        _redisCache.SetAsync($"order:{order.Id}", order)
                    );
                    await Task.WhenAll(tasks);
                    Console.WriteLine($"Total {updatedOrders.Count} new/updated orders synced to Redis.");
                }

                // Step 4: Handle **deleted records**
                var staleRedisKeys = redisOrderIds
                    .Where(redisId => !commandStoreOrderIds.Contains(redisId))
                    .Select(id => $"order:{id}")
                    .ToList();

                if (staleRedisKeys.Any())
                {
                    await _redisCache.DeleteBatchAsync(staleRedisKeys);
                    Console.WriteLine($"Total {staleRedisKeys.Count} stale records deleted from Redis.");
                }

                Console.WriteLine("Reconciliation completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reconciliation failed: {ex.Message}");
                // Log the error if using a logging service
            }
        }
    }
}
