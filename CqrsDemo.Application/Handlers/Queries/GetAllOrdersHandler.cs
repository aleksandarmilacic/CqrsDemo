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
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDTO>>
    {
        private readonly OrderService _orderService; 
        private readonly IMapper _mapper;

        public GetAllOrdersHandler(OrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDTO>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<IEnumerable<OrderDTO>>(await _orderService.GetAllOrdersAsync());
        }
    }
}
