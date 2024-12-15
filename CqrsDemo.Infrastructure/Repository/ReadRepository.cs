using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CqrsDemo.Infrastructure.Repository
{
    public class ReadRepository<T> : IReadRepository<T> where T : Entity<Guid>
    {
        private readonly ReadDbContext _dbContext;

        public ReadDbContext DbContext => _dbContext;

        public ReadRepository(ReadDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get entity by ID (deferred execution)
        public IQueryable<T> GetById(Guid id)
        {
            return _dbContext.Set<T>().Where(e => EF.Property<Guid>(e, "Id") == id);
        }
        public IQueryable<T> GetByIdAsNoTracking(Guid id)
        {
            return _dbContext.Set<T>().Where(e => EF.Property<Guid>(e, "Id") == id).AsNoTracking();
        }

        // Get all entities (deferred execution)
        public IQueryable<T> GetAll()
        {
            return _dbContext.Set<T>();
        }

        // Add a new entity (without saving)
        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        // Update an entity (without saving)
        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        // Delete an entity by ID (without saving)
        public void Delete(Guid id)
        {
            var entity = _dbContext.Set<T>().FirstOrDefault(e => EF.Property<Guid>(e, "Id") == id);
            if (entity != null)
            {
                _dbContext.Set<T>().Remove(entity);
            }
        }

        // Save changes explicitly
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
