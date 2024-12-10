using AutoMapper;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Application.Queries;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Queries
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDTO>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public GetOrderByIdHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDTO> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<OrderDTO>(await _context.Orders.SingleOrDefaultAsync(a => a.Id == request.Id));
        }
    }
}
