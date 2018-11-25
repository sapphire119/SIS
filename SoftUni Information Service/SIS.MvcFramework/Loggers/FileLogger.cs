namespace SIS.MvcFramework.Loggers
{
    using SIS.MvcFramework.Loggers.Contracts;

    using System;
    using System.IO;

    public class FileLogger : ILogger
    {
        private static readonly object LockObject = new object();

        private readonly string path;

        public FileLogger()
        {
            this.path = "log.txt";
        }

        public void Log(string message)
        {
            lock (LockObject)
            {
                File.AppendAllText(this.path, string.Concat(message, Environment.NewLine));
            }
        }
    }
}
