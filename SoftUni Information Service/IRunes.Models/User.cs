namespace IRunes.Models
{
    using System;

    public class User : BaseModel<Guid>
    {
        public User() { }

        public User(string username, string password, string email)
            : this()
        {
            this.Username = username;
            this.Password = password;
            this.Email = email;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
    }
}
