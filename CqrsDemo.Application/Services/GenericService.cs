using AutoMapper;
using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IGenericService<T, TDTO>> _logger;
        private readonly string _exchangeName;

        public GenericService(
            IWriteRepository<T> writeRepository,
            IReadRepository<T> readRepository,
            IRabbitMQPublisher rabbitMQPublisher,
            IMapper mapper,
            ILogger<IGenericService<T, TDTO>> logger)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _rabbitMQPublisher = rabbitMQPublisher;
            _mapper = mapper;
            _logger = logger;
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

                await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.created", entity);

                return _mapper.Map<TDTO>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {EntityName}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<TDTO> UpdateAsync(Guid id, T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");

            try
            {
                var existingEntity = await _writeRepository.GetAll().FirstOrDefaultAsync(a => a.Id == id);
                if (existingEntity == null)
                    throw new KeyNotFoundException($"Entity with ID {id} not found.");

                _writeRepository.Update(entity);
                await _writeRepository.SaveChangesAsync();
                await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.updated", entity);

                return _mapper.Map<TDTO>(entity);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException)
            {
                _logger.LogError(ex, "Error updating {EntityName} with ID {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _writeRepository.GetByIdAsNoTracking(id).FirstOrDefaultAsync();
                if (entity == null)
                    throw new KeyNotFoundException($"Entity with ID {id} not found.");

                _writeRepository.Delete(id);
                await _writeRepository.SaveChangesAsync();
                await _rabbitMQPublisher.PublishAsync(_exchangeName, $"{typeof(T).Name.ToLower()}.deleted", new { Id = id });
            }
            catch (Exception ex) when (ex is not KeyNotFoundException)
            {
                _logger.LogError(ex, "Error deleting {EntityName} with ID {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task<TDTO> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _readRepository.GetByIdAsNoTracking(id).FirstOrDefaultAsync();
                if (entity == null)
                    throw new KeyNotFoundException($"Entity with ID {id} not found.");

                return _mapper.Map<TDTO>(entity);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException)
            {
                _logger.LogError(ex, "Error fetching {EntityName} with ID {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<TDTO>> GetAllAsync()
        {
            try
            {
                var list = await GetQuery().ToListAsync();
                return _mapper.Map<IEnumerable<TDTO>>(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all {EntityName} entities", typeof(T).Name);
                throw;
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
                _logger.LogError(ex, "Error fetching queryable for {EntityName}", typeof(T).Name);
                throw;
            }
        }
    }

}
