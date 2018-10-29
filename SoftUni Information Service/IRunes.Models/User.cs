namespace IRunes.Models
{
    using System;

    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();


    }
}
