using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Domain.Entities;
using MediatR;

namespace CqrsDemo.Application.Commands.Order
{
    public class UpdateOrderCommand : UpdateCommand<CqrsDemo.Domain.Entities.Order.Order, OrderDTO>
    {
        public UpdateOrderCommand(Guid id, CqrsDemo.Domain.Entities.Order.Order entity) : base(id, entity)
        {
        }
    }
}
