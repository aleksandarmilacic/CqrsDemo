using AutoMapper;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Application.Queries;
using CqrsDemo.Application.Services.OrderServices;
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

        public GetAllOrdersHandler(OrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IEnumerable<OrderDTO>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _orderService.GetAllAsync();
        }
    }
}
