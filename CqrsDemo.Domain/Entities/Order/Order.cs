using System;

namespace CqrsDemo.Domain.Entities.Order
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        private Order() { }

        public Order(string name, decimal price)
        {
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

        public void UpdateWithModifiedDate(string name, decimal price, DateTime modifiedDate)
        {
            Name = name;
            Price = price;
            ModifiedDate = modifiedDate;
        }
    }
}

