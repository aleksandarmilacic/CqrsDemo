using AutoMapper;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Handlers
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDTO>
    {
        private readonly AppDbContext _context; 
        private readonly IMapper _mapper;

        public CreateOrderHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.Name, request.Price);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<OrderDTO>(order);
        }
    }
}
