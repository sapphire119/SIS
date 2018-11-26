﻿namespace SIS.MvcFramework.Services
{
    using SIS.MvcFramework.Loggers.Contracts;
    using SIS.MvcFramework.Services.Contracts;

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class UserCookieService : IUserCookieService
    {
        private readonly ILogger logger;

        public UserCookieService(ILogger logger)
        {
            this.logger = logger;
        }

        private const string EncryptKey =
            "E546C8DF278CD5931069B522E695D9F2";

        public string GetUserCookie(string username)
        {
            var encryptedUser = this.EncryptString(username, EncryptKey);
            return encryptedUser;
        }

        public string GetUserData(string cookieContent)
        {
            this.logger.Log("GetUserData()" + cookieContent);
            var username = this.DecryptString(cookieContent, EncryptKey);
            return username;
        }

        public string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }
}