﻿using AutoMapper;
using CqrsDemo.Application.Commands.Order;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Application.Services.OrderServices;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Commands.Order
{
    public class UpdateOrderHandler : UpdateCommandHandler<Domain.Entities.Order.Order, OrderDTO>, IBaseHandler
    {
       
        public UpdateOrderHandler(OrderService orderService) : base(orderService)
        { 
        }
 
    }
}
