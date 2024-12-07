using CqrsDemo.Application.DTOs;
using CqrsDemo.Domain.Entities;
using MediatR;

namespace CqrsDemo.Application.Queries
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<OrderDTO>>
    {
    }
}
