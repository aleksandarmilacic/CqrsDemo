using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Application.Services;
using CqrsDemo.Infrastructure.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Handlers.Commands
{
    public abstract class CreateCommandHandler<T, TDTO> : IRequestHandler<CreateCommand<T, TDTO>, TDTO> 
        where T : class
        where TDTO : IDTO
    {
        private readonly IGenericService<T, TDTO> _service;

        public CreateCommandHandler(IGenericService<T, TDTO> service)
        {
            _service = service;
        }

        public async Task<TDTO> Handle(CreateCommand<T, TDTO> request, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(request.Entity);
        }
    }


    public abstract class UpdateCommandHandler<T, TDTO> : IRequestHandler<UpdateCommand<T, TDTO>, TDTO> 
        where T : class
        where TDTO : IDTO
    {
        private readonly IGenericService<T, TDTO> _service;

        public UpdateCommandHandler(IGenericService<T, TDTO> service)
        {
            _service = service;
        }

        public async Task<TDTO> Handle(UpdateCommand<T, TDTO> request, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(request.Id, request.Entity);
        }
    }

    public abstract class DeleteCommandHandler<T, TDTO> : IRequestHandler<DeleteCommand<T>, Unit>  
        where T : class
        where TDTO : IDTO
    {
        private readonly IGenericService<T, TDTO> _service;

        public DeleteCommandHandler(IGenericService<T, TDTO> service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DeleteCommand<T> request, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }

}
