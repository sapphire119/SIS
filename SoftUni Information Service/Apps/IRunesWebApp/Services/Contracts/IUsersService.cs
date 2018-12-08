namespace IRunesWebApp.Services.Contracts
{
    using IRunesWebApp.ViewModels.Users;

    public interface IUsersService
    {
        bool ExistsByUsernameAndPassword(string username, string password);

        void AddUserToContext(RegisterUserViewModel username);
    }
}
