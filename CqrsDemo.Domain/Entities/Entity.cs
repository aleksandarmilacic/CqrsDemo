using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Domain.Entities
{
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Entity<TKey> entity)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (IsTransient() || entity.IsTransient())
            {
                return false;
            }

            return EqualityComparer<TKey>.Default.Equals(Id, entity.Id);
        }

        public override int GetHashCode()
        {
            return !IsTransient()
                ? Id.GetHashCode()
                : base.GetHashCode();
        }

        public bool IsTransient()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default);
        }
    }

}
