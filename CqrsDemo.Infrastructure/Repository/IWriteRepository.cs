using CqrsDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Infrastructure.Repository
{
    public interface IWriteRepository<T> where T : IEntity<Guid>
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
