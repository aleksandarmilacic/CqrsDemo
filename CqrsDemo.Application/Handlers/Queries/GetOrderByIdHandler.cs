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
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDTO>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly RedisCache _redisCache;

        public GetOrderByIdHandler(AppDbContext context, IMapper mapper, RedisCache redisCache)
        {
            _context = context;
            _mapper = mapper;
            _redisCache = redisCache;
        }

        public async Task<OrderDTO> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<OrderDTO>(await _redisCache.GetAsync<Order>($"order:{request.Id}"));
        }
    }
}
