using AutoMapper;
using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities;
using MediatR;

namespace CqrsDemo.Application.Handlers.Commands
{
    public abstract class UpdateCommandHandler<TCommand, TEntity, TDTO> : IRequestHandler<TCommand, TDTO>
      where TCommand : IRequest<TDTO>, IEntity<Guid>
      where TEntity : IEntity<Guid>
      where TDTO : IDTO
    {
        private readonly IGenericService<TEntity, TDTO> _service;
        private readonly IMapper _mapper;

        public UpdateCommandHandler(IGenericService<TEntity, TDTO> service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<TDTO> Handle(TCommand request, CancellationToken cancellationToken)
        {
            // Map updated properties from the command to the entity
            var entity = _mapper.Map<TEntity>(request);
            return await _service.UpdateAsync(request.Id, entity);
        }
    }


}
