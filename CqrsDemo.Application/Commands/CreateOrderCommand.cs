using CqrsDemo.Application.DTOs;
using CqrsDemo.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Commands
{
    public class CreateOrderCommand : IRequest<OrderDTO>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
