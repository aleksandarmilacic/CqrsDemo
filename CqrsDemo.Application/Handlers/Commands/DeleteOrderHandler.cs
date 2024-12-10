using CqrsDemo.Application.Commands;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Commands
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly AppDbContext _context;

        public DeleteOrderHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
            if (order == null)
            {
                return Unit.Value; // Return Unit.Value when no action is performed
            }

            // in reality this would be a soft delete
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value; // Always return Unit.Value for void-like responses
        }
    }
}
