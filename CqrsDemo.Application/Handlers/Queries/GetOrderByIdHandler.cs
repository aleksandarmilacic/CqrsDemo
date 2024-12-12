using AutoMapper;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Application.Queries;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Caching;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Queries
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDTO>
    {
        private readonly OrderService _orderService;
        private readonly IMapper _mapper;

        public GetOrderByIdHandler(OrderService context, IMapper mapper)
        {
            _orderService = context;
            _mapper = mapper;
        }

        public async Task<OrderDTO> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<OrderDTO>(await _orderService.GetOrderByIdAsync(request.Id));
        }
    }
}
