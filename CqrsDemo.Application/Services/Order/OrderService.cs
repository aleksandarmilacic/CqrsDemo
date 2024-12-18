using AutoMapper;
using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = CqrsDemo.Domain.Entities.Order.Order;

namespace CqrsDemo.Application.Services.OrderServices
{
    public interface IOrderService : IGenericService<Order, OrderDTO>
    {
    }
    public class OrderService : GenericService<Order, OrderDTO>, IOrderService
    {
        public OrderService(IWriteRepository<Order> writeRepository, 
            IReadRepository<Order> readRepository,
        IRabbitMQPublisher rabbitMQPublisher, 
            IMapper mapper,
            ILogger<IGenericService<Order, OrderDTO>> logger) 
            : base(writeRepository, readRepository, rabbitMQPublisher, mapper, logger)
        {
        } 
    }

}
