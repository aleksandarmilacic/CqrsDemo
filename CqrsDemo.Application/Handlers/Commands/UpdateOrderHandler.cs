using AutoMapper;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.DTOs;
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

        public UpdateOrderHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDTO> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
            if (order == null) return null;

            order.Update(request.Name, request.Price);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<OrderDTO>(order);
        }
    }
}
