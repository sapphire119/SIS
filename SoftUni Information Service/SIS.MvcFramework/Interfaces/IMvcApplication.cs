namespace SIS.MvcFramework.Interfaces
{
    using SIS.MvcFramework.Services.Contracts;

    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices(IServiceCollection serviceCollection);
    }
}