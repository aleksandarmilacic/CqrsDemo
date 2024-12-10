using AutoMapper;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Application.Queries;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Caching;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Queries
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDTO>>
    {
        private readonly AppDbContext _context;
        private readonly RedisCache _redisCache;
        private readonly IMapper _mapper;
        public GetAllOrdersHandler(AppDbContext context, RedisCache redisCache, IMapper mapper)
        {
            _context = context;
            _redisCache = redisCache;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDTO>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<IEnumerable<OrderDTO>>(await _redisCache.GetAllAsync<Order>("order:*"));
        }
    }
}
