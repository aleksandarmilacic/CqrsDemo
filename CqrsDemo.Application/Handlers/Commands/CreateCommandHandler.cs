using AutoMapper;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Handlers.Commands
{
    public abstract class CreateCommandHandler<TCommand, TEntity, TDTO> : IRequestHandler<TCommand, TDTO>
    where TCommand : IRequest<TDTO>
    where TEntity : IEntity<Guid>
    where TDTO : IDTO
    {
        private readonly IGenericService<TEntity, TDTO> _service;
        private readonly IMapper _mapper;

        public CreateCommandHandler(IGenericService<TEntity, TDTO> service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<TDTO> Handle(TCommand request, CancellationToken cancellationToken)
        {
            // Map properties from command to entity
            var entity = _mapper.Map<TEntity>(request);
            return await _service.CreateAsync(entity);
        }
    }



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
