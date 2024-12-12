using CqrsDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CqrsDemo.Infrastructure.Repository
{
    public class WriteRepository<T> : IRepository<T> where T : class
    {
        private readonly WriteDbContext _dbContext;

        public WriteRepository(WriteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get entity by ID
        public IQueryable<T> GetById(Guid id)
        {
            return _dbContext.Set<T>().Where(e => EF.Property<Guid>(e, "Id") == id);
        }

        // Get all entities
        public IQueryable<T> GetAll()
        {
            return _dbContext.Set<T>();
        }

        // Add a new entity
        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        // Update an existing entity
        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        // Delete an entity by ID
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
