using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Domain.Entities;
using MediatR;

namespace CqrsDemo.Application.Queries
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<OrderDTO>>
    {
    }
}
