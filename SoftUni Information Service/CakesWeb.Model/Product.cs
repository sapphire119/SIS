namespace CakesWeb.Models
{
    using System.Collections.Generic;

    public class Product : BaseModel<int>
    {
        public Product()
        {
            this.Orders = new HashSet<OrderProduct>();
        }

        public Product(string name, decimal price, string imageUrl)
            : this()
        {
            this.Name = name;
            this.Price = price;
            this.ImageUrl = imageUrl;
        }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<OrderProduct> Orders { get; set; }
    }
}
