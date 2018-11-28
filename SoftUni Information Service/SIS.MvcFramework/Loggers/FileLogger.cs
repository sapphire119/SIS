namespace SIS.MvcFramework.Loggers
{
    using SIS.MvcFramework.Loggers.Contracts;

    using System;
    using System.IO;

    public class FileLogger : ILogger
    {
        private static readonly object LockObject = new object();

        private readonly string path;

        public FileLogger(string path)
        {
            this.path = path;
        }

        public void Log(string message)
        {
            lock (LockObject)
            {
                File.AppendAllText(this.path, message + Environment.NewLine); /*string.Concat($"[{DateTime.UtcNow}] ",message, Environment.NewLine)*/
            }
        }
    }
}
