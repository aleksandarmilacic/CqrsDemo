namespace CqrsDemo.Domain.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }

        bool Equals(object obj);
        int GetHashCode();
         
    }
}