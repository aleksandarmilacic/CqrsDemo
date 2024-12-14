using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Commands.Order;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Application.Services.OrderServices;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Application.Handlers.Commands.Order
{
    public class DeleteOrderHandler : DeleteCommandHandler<Domain.Entities.Order.Order, OrderDTO>, IBaseHandler
    {
      
        public DeleteOrderHandler(OrderService orderService) : base(orderService)
        { 
        }
    }
}
