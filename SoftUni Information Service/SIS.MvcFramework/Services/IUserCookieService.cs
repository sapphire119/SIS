namespace SIS.MvcFramework.Services
{
    public interface IUserCookieService
    {
        string DecryptString(string cipherText, string keyString);

        string EncryptString(string text, string keyString);

        string GetUserCookie(string username);

        string GetUserData(string encryptedUser);
    }
}