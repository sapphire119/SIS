namespace IRunes.Models
{
    using System;
    using System.Collections.Generic;

    public class Album : BaseModel<Guid>
    {
        public Album()
        {
            this.Tracks = new HashSet<Track>();
        }

        public Album(string name, string cover, decimal price)
            : this()
        {
            this.Name = name;
            this.Cover = cover;
            this.Price = price;
        }

        public string Name { get; set; }

        public string Cover { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}
