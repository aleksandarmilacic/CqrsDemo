using AutoMapper;
using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Services
{
    public interface IGenericService<T, TDTO> 
        where T : IEntity<Guid> 
        where TDTO : IDTO
    {
        Task<TDTO> CreateAsync(T entity);
        Task<TDTO> UpdateAsync(Guid id, T entity);
        Task DeleteAsync(Guid id);
        Task<TDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<TDTO>> GetAllAsync();
        IQueryable<T> GetQuery();
    }

    public class GenericService<T, TDTO> : IGenericService<T, TDTO>
        where T : IEntity<Guid>
        where TDTO : IDTO
    {
        private readonly IWriteRepository<T> _writeRepository;
        private readonly IReadRepository<T> _readRepository;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        private readonly IMapper _mapper;
        private readonly string _exchangeName;

        public GenericService(
            IWriteRepository<T> writeRepository,
            IReadRepository<T> readRepository,
            IRabbitMQPublisher rabbitMQPublisher,
            IMapper mapper)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _rabbitMQPublisher = rabbitMQPublisher;
            _mapper = mapper;
            _exchangeName = $"{typeof(T).Name.ToLower()}-exchange";
        }

        public virtual async Task<TDTO> CreateAsync(T entity)
        {
            _writeRepository.Add(entity);
            await _writeRepository.SaveChangesAsync();

            // Publish event to RabbitMQ
            await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.created", entity);

            return _mapper.Map<TDTO>(entity);
        }

        public virtual async Task<TDTO> UpdateAsync(Guid id, T entity)
        {
            _writeRepository.Update(entity);
            await _writeRepository.SaveChangesAsync();

            await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.updated", entity);
            return _mapper.Map<TDTO>(entity);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            _writeRepository.Delete(id);
            await _writeRepository.SaveChangesAsync();

            await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.deleted", new { Id = id });
        }

        public virtual async Task<TDTO> GetByIdAsync(Guid id)
        {
            var query = _readRepository.GetById(id);
            var entity = await query.SingleOrDefaultAsync();
            return _mapper.Map<TDTO>(entity);
        }

        public virtual async Task<IEnumerable<TDTO>> GetAllAsync()
        {
            var query = GetQuery();
            var list = await query.ToListAsync();

            return _mapper.Map<IEnumerable<TDTO>>(list);
        }

        public virtual IQueryable<T> GetQuery()
        {
            return _readRepository.GetAll();
        }
    }
}
