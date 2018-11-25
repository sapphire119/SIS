namespace CakeWeb.App
{
    using SIS.MvcFramework.Interfaces;
    using SIS.MvcFramework.Loggers;
    using SIS.MvcFramework.Loggers.Contracts;
    using SIS.MvcFramework.Services;
    using SIS.MvcFramework.Services.Contracts;


    public class StartUp : IMvcApplication
    {
        public void Configure()
        {
            return;
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IHashService, HashService>();
            collection.AddService<IUserCookieService, UserCookieService>();
            collection.AddService<ILogger, FileLogger>();
            // TODO: Implement IoC/DI(Dependency Injection) Container (Inversion of Control)

            return;
        }
    }
}
