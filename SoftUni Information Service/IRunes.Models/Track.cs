namespace IRunes.Models
{
    using System;

    public class Track : BaseModel<Guid>
    {
        public Track() { }

        public Track(string name, string link, decimal price, Guid albumId)
            :this()
        {
            this.Name = name;
            this.Link = link;
            this.Price = price;
            this.AlbumId = albumId;
        }

        public string Name { get; set; }

        public string Link { get; set; }

        public decimal Price { get; set; }

        public Guid AlbumId { get; set; }

        public virtual Album Album { get; set; }
    }
}
