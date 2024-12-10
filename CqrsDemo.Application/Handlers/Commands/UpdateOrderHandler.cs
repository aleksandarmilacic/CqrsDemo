using AutoMapper;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Commands
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, OrderDTO>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;

        public UpdateOrderHandler(AppDbContext context, IMapper mapper, IRabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            _mapper = mapper;
            this._rabbitMQPublisher = rabbitMQPublisher;
        }

        public async Task<OrderDTO> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
            if (order == null) return null;

            order.Update(request.Name, request.Price);
            await _context.SaveChangesAsync(cancellationToken);

            // Publish the update event to RabbitMQ
            await _rabbitMQPublisher.PublishAsync(
                exchangeName: "order-exchange",
                routingKey: "order.updated",
                message: order
            );

            return _mapper.Map<OrderDTO>(order);
        }
    }
}
