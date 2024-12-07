using MediatR;

namespace CqrsDemo.Application.Commands
{
    public class DeleteOrderCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
