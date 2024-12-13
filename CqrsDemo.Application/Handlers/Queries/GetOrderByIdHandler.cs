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
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDTO>
    {
        private readonly OrderService _orderService;

        public GetOrderByIdHandler(OrderService context)
        {
            _orderService = context; 
        }

        public async Task<OrderDTO> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return await _orderService.GetByIdAsync(request.Id);
        }
    }
}
