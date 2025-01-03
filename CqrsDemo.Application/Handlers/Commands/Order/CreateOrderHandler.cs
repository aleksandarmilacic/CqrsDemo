﻿using AutoMapper;
using CqrsDemo.Application.Commands.Order;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Application.Services;
using CqrsDemo.Application.Services.OrderServices;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Handlers.Commands.Order
{
  

    public class CreateOrderHandler : CreateCommandHandler<CreateOrderCommand, Domain.Entities.Order.Order, OrderDTO>
    {
        public CreateOrderHandler(IOrderService service, IMapper mapper) : base(service, mapper)
        {
            
        }
    }
}
