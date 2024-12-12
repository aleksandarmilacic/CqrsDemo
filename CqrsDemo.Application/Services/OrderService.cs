using AutoMapper;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = CqrsDemo.Domain.Entities.Order;

namespace CqrsDemo.Application.Services
{
    public class OrderService
    {
        private readonly IWriteRepository<Order> _writeRepository;
        private readonly IReadRepository<Order> _readRepository;
        private readonly IMapper _mapper;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;

        public OrderService(IWriteRepository<Order> writeRepository, IReadRepository<Order> readRepository, IMapper mapper, IRabbitMQPublisher rabbitMQPublisher)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _mapper = mapper;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public async Task CreateOrderAsync(Order order)
        {
            _writeRepository.Add(order);
            await _writeRepository.SaveChangesAsync();

            // Publish event to RabbitMQ
            await _rabbitMQPublisher.PublishAsync("order-exchange", "order.created", order);
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            var query = _readRepository.GetById(id);
            return await query.SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var query = _readRepository.GetAll();
            return await query.ToListAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _writeRepository.Update(order);
            await _writeRepository.SaveChangesAsync();

            // Publish the update event to RabbitMQ
            await _rabbitMQPublisher.PublishAsync(
                exchangeName: "order-exchange",
                routingKey: "order.updated",
                message: order
            );
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            var order = await GetOrderByIdAsync(id);
            if (order != null)
            {
                _writeRepository.Delete(order.Id);
                await _writeRepository.SaveChangesAsync();
            }

            await _rabbitMQPublisher.PublishAsync(
               exchangeName: "order-exchange",
               routingKey: "order.deleted",
               message: order
           );
        }
    }

}
