using AutoMapper;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Commands
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, OrderDTO>
    {
        private readonly OrderService _orderService;
        private readonly IMapper _mapper;

        public UpdateOrderHandler(OrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<OrderDTO> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetOrderByIdAsync(request.Id);
            if (order == null) return null;

            order.Update(request.Name, request.Price);
            await _orderService.UpdateOrderAsync(order);

            return _mapper.Map<OrderDTO>(order);
        }
    }
}
