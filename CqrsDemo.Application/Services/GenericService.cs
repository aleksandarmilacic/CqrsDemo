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
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");

            try
            {
                _writeRepository.Add(entity);
                await _writeRepository.SaveChangesAsync();

                // Publish event to RabbitMQ
                await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.created", entity);

                return _mapper.Map<TDTO>(entity);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException($"Error creating {typeof(T).Name}", ex);
            }
        }

        public virtual async Task<TDTO> UpdateAsync(Guid id, T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");

            try
            {
                var exists = await _writeRepository.GetAll().AnyAsync(a => a.Id == id);
                if (!exists)
                    throw new KeyNotFoundException($"Entity with ID {id} not found.");

                _writeRepository.Update(entity);
                await _writeRepository.SaveChangesAsync();

                await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.updated", entity);

                return _mapper.Map<TDTO>(entity);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException($"Error updating {typeof(T).Name} with ID {id}", ex);
            }
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            try
            {
                var exists = await _writeRepository.GetByIdAsNoTracking(id).AnyAsync();
                if (!exists)
                    throw new KeyNotFoundException($"Entity with ID {id} not found.");

                _writeRepository.Delete(id);
                await _writeRepository.SaveChangesAsync();

                await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.deleted", new { Id = id });
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException($"Error deleting {typeof(T).Name} with ID {id}", ex);
            }
        }

        public virtual async Task<TDTO> GetByIdAsync(Guid id)
        {
            try
            {
                var query = _readRepository.GetByIdAsNoTracking(id);
                var entity = await query.SingleOrDefaultAsync();

                if (entity == null)
                    throw new KeyNotFoundException($"Entity with ID {id} not found.");

                return _mapper.Map<TDTO>(entity);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException($"Error fetching {typeof(T).Name} with ID {id}", ex);
            }
        }

        public virtual async Task<IEnumerable<TDTO>> GetAllAsync()
        {
            try
            {
                var query = GetQuery();
                var list = await query.ToListAsync();

                return _mapper.Map<IEnumerable<TDTO>>(list);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException($"Error fetching all {typeof(T).Name} entities", ex);
            }
        }

        public virtual IQueryable<T> GetQuery()
        {
            try
            {
                return _readRepository.GetAll();
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException($"Error fetching queryable for {typeof(T).Name}", ex);
            }
        }
    }
}
