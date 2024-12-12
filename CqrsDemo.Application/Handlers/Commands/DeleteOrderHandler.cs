using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Services;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Commands
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly OrderService _orderService;

        public DeleteOrderHandler(OrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            await _orderService.DeleteOrderAsync(request.Id);

            return Unit.Value; 
        }
    }
}
