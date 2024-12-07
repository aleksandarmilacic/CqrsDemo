using CqrsDemo.Application.Commands;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, Order>
    {
        private readonly AppDbContext _context;

        public UpdateOrderHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
            if (order == null) return null;

            order.Update(request.Name, request.Price);
            await _context.SaveChangesAsync(cancellationToken);

            return order;
        }
    }
}
