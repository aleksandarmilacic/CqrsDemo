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

        public async Task ReconcileAsync()
        {
            var commandStoreOrders = await _dbContext.Orders.ToListAsync();

            foreach (var order in commandStoreOrders)
            {
                var cachedOrder = await _redisCache.GetAsync<Order>($"order:{order.Id}");
                if (cachedOrder == null)
                {
                    await _redisCache.SetAsync($"order:{order.Id}", order);
                }
            }
        }
    }
}
