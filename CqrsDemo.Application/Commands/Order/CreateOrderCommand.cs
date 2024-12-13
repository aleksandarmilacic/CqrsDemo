using CqrsDemo.Application.Handlers.Commands;
using CqrsDemo.Application.Models.DTOs.Order; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Commands.Order
{
    public class CreateOrderCommand : CreateCommand<CqrsDemo.Domain.Entities.Order.Order, OrderDTO>
    {
        public CreateOrderCommand(CqrsDemo.Domain.Entities.Order.Order entity) : base(entity)
        {
        }
    }
    
}
