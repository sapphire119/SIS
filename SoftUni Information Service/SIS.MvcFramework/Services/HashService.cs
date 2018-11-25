namespace SIS.MvcFramework.Services
{
    using SIS.MvcFramework.Loggers.Contracts;
    using SIS.MvcFramework.Services.Contracts;

    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class HashService : IHashService
    {
        private readonly ILogger logger;

        public HashService(ILogger logger)
        {
            this.logger = logger;
        }

        public string Hash(string stringToHash)
        {

            stringToHash = stringToHash + "myAppSalt8923467283461#";
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                this.logger.Log(hash);

                return hash;
            }
        }
    }
}
