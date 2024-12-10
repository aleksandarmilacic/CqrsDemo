using AutoMapper;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Handlers.Commands
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDTO>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;

        public CreateOrderHandler(AppDbContext context, IMapper mapper, IRabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.Name, request.Price);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);


            // Publish event to RabbitMQ
            await _rabbitMQPublisher.PublishAsync("order-exchange", "order.created", order);


            return _mapper.Map<OrderDTO>(order);
        }
    }
}
