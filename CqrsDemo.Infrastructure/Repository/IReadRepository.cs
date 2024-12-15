using CqrsDemo.Domain.Entities;
using CqrsDemo.Infrastructure.Persistence;

namespace CqrsDemo.Infrastructure.Repository
{
    public interface IReadRepository<T> where T : IEntity<Guid>
    {
        ReadDbContext DbContext { get; }
        void Add(T entity);
        void Delete(Guid id);
        IQueryable<T> GetAll();
        IQueryable<T> GetById(Guid id);
        IQueryable<T> GetByIdAsNoTracking(Guid id);
        void SaveChanges();
        Task SaveChangesAsync();
        void Update(T entity);
    }
}
