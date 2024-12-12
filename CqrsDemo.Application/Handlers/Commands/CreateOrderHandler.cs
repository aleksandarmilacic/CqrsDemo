using AutoMapper;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.DTOs;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Handlers.Commands
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDTO>
    {
        private readonly OrderService _orderService;
        private readonly IMapper _mapper; 

        public CreateOrderHandler(OrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper; 
        }

        public async Task<OrderDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.Name, request.Price); 

            await _orderService.CreateOrderAsync(order);

            return _mapper.Map<OrderDTO>(order);
        }
    }
}
