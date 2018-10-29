namespace IRunes.Models
{
    using System;

    public class Track : BaseModel<Guid>
    {
        public string Name { get; set; }

        public string Link { get; set; }

        public decimal Price { get; set; }
    }
}
