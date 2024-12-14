using CqrsDemo.Application.Models.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Commands
{
    public abstract class CreateCommand<T, TDTO> : IRequest<TDTO> 
        where T : class 
        where TDTO : IDTO
    {
        public T Entity { get; set; }

        public CreateCommand(T entity)
        {
            Entity = entity;
        }
    }

    public abstract class UpdateCommand<T, TDTO> : IRequest<TDTO>
        where T : class
        where TDTO : IDTO
    {
        public Guid Id { get; set; }
        public T Entity { get; set; }

        public UpdateCommand(Guid id, T entity)
        {
            Id = id;
            Entity = entity;
        }
    }

    public abstract class DeleteCommand<T> : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeleteCommand(Guid id)
        {
            Id = id;
        }
    }

}
