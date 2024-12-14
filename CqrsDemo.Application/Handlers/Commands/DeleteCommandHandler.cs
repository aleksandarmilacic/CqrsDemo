using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities;
using MediatR;

namespace CqrsDemo.Application.Handlers.Commands
{
    public abstract class DeleteCommandHandler<TCommand, TEntity, TDTO> : IRequestHandler<TCommand, Unit>
     where TCommand : IRequest<Unit>, IEntity<Guid>
     where TEntity : IEntity<Guid>
     where TDTO : IDTO
    {
        private readonly IGenericService<TEntity, TDTO> _service;

        public DeleteCommandHandler(IGenericService<TEntity, TDTO> service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }


}
