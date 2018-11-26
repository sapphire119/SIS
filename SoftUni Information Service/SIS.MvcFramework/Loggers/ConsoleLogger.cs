namespace SIS.MvcFramework.Loggers
{
    using SIS.MvcFramework.Loggers.Contracts;

    using System;

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[{DateTime.UtcNow}] {message}");
        }
    }
}
