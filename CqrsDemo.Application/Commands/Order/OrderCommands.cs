using CqrsDemo.Application.Handlers.Commands;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Commands.Order
{
     

    public record CreateOrderCommand(string Name, decimal Price) : IRequest<OrderDTO>;
    public record DeleteOrderCommand(Guid Id) : IEntity<Guid>, IRequest<Unit>;
    public record UpdateOrderCommand(Guid Id, string Name, decimal Price) : IEntity<Guid>, IRequest<OrderDTO>;
}
