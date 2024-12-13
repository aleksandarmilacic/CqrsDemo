using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDTO>
    {
        public Guid Id { get; set; }
    }
}
