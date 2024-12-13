using MediatR;

namespace CqrsDemo.Application.Commands.Order
{
    public class DeleteOrderCommand : DeleteCommand<Domain.Entities.Order.Order>
    {
        public DeleteOrderCommand(Guid id) : base(id)
        {
        }

    }
}
