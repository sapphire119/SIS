namespace IRunesWebApp.Services
{
    using System;
    using System.Linq;
    using global::Services;
    using IRunesWebApp.Data;
    using IRunesWebApp.Models;
    using IRunesWebApp.Services.Contracts;
    using IRunesWebApp.ViewModels.Users;

    using Services;

    public class UsersService : IUsersService
    {
        private readonly IRunesContext context;
        private readonly IHashService hashService;

        public UsersService(IRunesContext context, IHashService hashService)
        {
            this.context = context;
            this.hashService = hashService;
        }

        public bool ExistsByUsernameAndPassword(string username, string password)
        {
            var hashedPassword = this.hashService.Hash(password);

            var userExists = this.context.Users
                .Any(u => u.Username == username &&
                          u.HashedPassword == hashedPassword);

            return userExists;
        }

        public void AddUserToContext(RegisterUserViewModel user)
        {
            if (this.context.Users.Any(u => u.Username == user.Username))
            {
                throw new Exception("User already exits!");
            }

            var userForDb = new User()
            {
                
            }

            this.context.Users.Add(user);
        }
    }
}
