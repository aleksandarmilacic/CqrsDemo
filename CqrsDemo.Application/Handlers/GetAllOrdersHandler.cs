using AutoMapper;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Application.Queries;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDTO>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public GetAllOrdersHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper; 
        }

        public async Task<IEnumerable<OrderDTO>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<IEnumerable<OrderDTO>>(await _context.Orders.ToListAsync(cancellationToken));
        }
    }
}
