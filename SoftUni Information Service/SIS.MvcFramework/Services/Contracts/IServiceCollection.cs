using System;

namespace SIS.MvcFramework.Services.Contracts
{
    public interface IServiceCollection
    {
        void AddService<TSource, TDestination>();

        T CreateInstace<T>();

        object CreateInstance(Type type);
    }
}
