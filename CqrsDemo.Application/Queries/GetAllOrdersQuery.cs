using CqrsDemo.Domain.Entities;
using MediatR;

namespace CqrsDemo.Application.Queries
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<Order>>
    {
    }
}
