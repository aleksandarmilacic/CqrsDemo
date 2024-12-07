using CqrsDemo.Application.DTOs;
using CqrsDemo.Domain.Entities;
using MediatR;

namespace CqrsDemo.Application.Commands
{
    public class UpdateOrderCommand : IRequest<OrderDTO>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
