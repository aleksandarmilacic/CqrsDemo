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
        private readonly WriteDbContext _writeDbContext;
        private readonly ReadDbContext _readDbContext;

        public ReconciliationService(WriteDbContext writeDbContext, ReadDbContext readDbContext)
        {
           _writeDbContext = writeDbContext;
           _readDbContext = readDbContext;
        }

        /// <summary>
        /// Reconcile the Write Store (MSSQL) with the Read Store (PostgreSQL).
        /// </summary>
        /// <param name="takeSince">The DateTime to sync changes from.</param>
        public async Task ReconcileAsync(DateTime takeSince)
        {
            try
            {
                // Step 1: Get all orders from the Write DB that changed since `takeSince`
                var commandStoreOrders = await _writeDbContext.Orders
                    .AsNoTracking()
                    .Where(o => o.CreatedDate >= takeSince || o.ModifiedDate >= takeSince)
                    .ToListAsync();

                // Step 2: Get all orders from the Read DB
                var readStoreOrders = await _readDbContext.Orders.AsNoTracking().ToListAsync();
                var readStoreOrderIds = readStoreOrders.Select(o => o.Id).ToList();

                // Step 3: Handle **new and updated** records
                var updatedOrders = commandStoreOrders
                    .Where(order => !readStoreOrderIds.Contains(order.Id) ||
                                    readStoreOrders.Any(ro => ro.Id == order.Id && ro.ModifiedDate < order.ModifiedDate))
                    .ToList();

                if (updatedOrders.Any())
                {
                    foreach (var order in updatedOrders)
                    {
                        var existingOrder = await _readDbContext.Orders.FirstOrDefaultAsync(o => o.Id == order.Id);
                        if (existingOrder != null)
                        {
                            // Update existing record
                            existingOrder.UpdateWithModifiedDate(order.Name, order.Price, order.ModifiedDate);
                        }
                        else
                        {
                            // Add new record
                            await _readDbContext.Orders.AddAsync(order);
                        }
                    }

                    await _readDbContext.SaveChangesAsync();
                    Console.WriteLine($"Total {updatedOrders.Count} new/updated orders synced to Read DB.");
                }

                // Step 4: Handle **deleted records**
                var commandStoreOrderIds = commandStoreOrders.Select(o => o.Id).ToList();
                var staleOrders = readStoreOrders
                    .Where(readOrder => !commandStoreOrderIds.Contains(readOrder.Id))
                    .ToList();

                if (staleOrders.Any())
                {
                    _readDbContext.Orders.RemoveRange(staleOrders);
                    await _readDbContext.SaveChangesAsync();
                    Console.WriteLine($"Total {staleOrders.Count} stale records deleted from Read DB.");
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
