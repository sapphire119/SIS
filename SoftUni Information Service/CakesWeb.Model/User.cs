namespace CakesWeb.Models
{
    using System;
    using System.Collections.Generic;

    public class User : BaseModel<int>
    {
        public User()
        {
            this.Orders = new HashSet<Order>();
        }

        public User(string name, string username, string password)
            : this()
        {
           this.Name = name;
           this.Username = username;
           this.Password = password;
        }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
