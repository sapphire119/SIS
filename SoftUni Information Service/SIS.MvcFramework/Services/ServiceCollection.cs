namespace SIS.MvcFramework.Services
{
    using SIS.MvcFramework.Services.Contracts;
    using System;
    using System.Collections.Generic;

    public class ServiceCollection : IServiceCollection
    {
        private IDictionary<Type, Type> dependencyContainer;

        public ServiceCollection()
        {
            this.dependencyContainer = new Dictionary<Type, Type>();
        }

        public void AddService<TSource, TDestination>()
        {
            this.dependencyContainer[typeof(TSource)] = typeof(TDestination);
        }
    }
}
