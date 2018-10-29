namespace IRunes.Services.Interfaces
{
    public interface IEncryptService
    {
        string EncryptString(string text, string keyString);

        string DecryptString(string cipherText, string keyString);
    }
}
