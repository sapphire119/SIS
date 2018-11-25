namespace SIS.MvcFramework.Services
{
    using SIS.MvcFramework.Services.Contracts;

    using System;
    using System.Linq;
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

        public T CreateInstace<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        public object CreateInstance(Type type)
        {
            if (this.dependencyContainer.ContainsKey(type))
            {
                type = this.dependencyContainer[type];
            }

            if (type.IsInterface || type.IsAbstract)
            {
                throw new Exception($"Type: {type.FullName} cannot be instantiated");
            }

            var constructor = type.GetConstructors().First();

            var parameters = constructor.GetParameters();

            List<Object> constructorParamterObject = new List<object>();

            foreach (var paramter in parameters)
            {
                var paramterObj = this.CreateInstance(paramter.ParameterType);

                constructorParamterObject.Add(paramterObj);
            }

            var obj =  constructor.Invoke(constructorParamterObject.ToArray());

            return obj;
        }
    }
}
