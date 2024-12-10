using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Services;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Commands
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly AppDbContext _context;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;

        public DeleteOrderHandler(AppDbContext context, IRabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            this._rabbitMQPublisher = rabbitMQPublisher;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
            if (order != null)
            {
                // in reality this would be a soft delete
                _context.Orders.Remove(order);

                await _context.SaveChangesAsync(cancellationToken);
            }

         

            // Publish the update event to RabbitMQ
            await _rabbitMQPublisher.PublishAsync(
                exchangeName: "order-exchange",
                routingKey: "order.deleted",
                message: order
            );
            return Unit.Value; // Always return Unit.Value for void-like responses
        }
    }
}
