namespace IRunes.Services
{
    using IRunes.Services.Interfaces;

    public class UserCookieService : IUserCookieService
    {
        private const string EncryptKey = "D546F8GF278CD5931069B522E695D9F2";

        private IEncryptService encryptService;

        public UserCookieService()
        {
            this.encryptService = new EncryptService();
        }

        public string GetUserCookie(string username)
        {
            var cookieContent = this.encryptService.EncryptString(username, EncryptKey);

            return cookieContent;
        }

        public string GetUserData(string cookieContent)
        {
            var username = this.encryptService.DecryptString(cookieContent, EncryptKey);

            return username;
        }
    }
}
