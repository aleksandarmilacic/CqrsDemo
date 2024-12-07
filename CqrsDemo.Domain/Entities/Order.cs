using System;

namespace CqrsDemo.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        private Order() { }

        public Order(string name, decimal price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = DateTime.UtcNow;
        }

        public void Update(string name, decimal price)
        {
            Name = name;
            Price = price;
            ModifiedDate = DateTime.UtcNow;
        }
    }
}

