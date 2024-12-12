namespace CqrsDemo.Infrastructure.Repository
{
    public interface IReadRepository<T> where T : class
    {
        void Add(T entity);
        void Delete(Guid id);
        IQueryable<T> GetAll();
        IQueryable<T> GetById(Guid id);
        void SaveChanges();
        Task SaveChangesAsync();
        void Update(T entity);
    }
}
