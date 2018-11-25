namespace SIS.MvcFramework.Services.Contracts
{
    public interface IServiceCollection
    {
        void AddService<TSource, TDestination>();
    }
}
