namespace SIS.MvcFramework.Services
{
    using SIS.MvcFramework.Services.Contracts;

    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class ServiceCollection : IServiceCollection
    {
        private readonly IDictionary<Type, Type> dependencyContainer;
        private readonly IDictionary<Type, Func<object>> dependencyContainerWithFunc;

        public ServiceCollection()
        {
            this.dependencyContainer = new Dictionary<Type, Type>();
            this.dependencyContainerWithFunc = new Dictionary<Type, Func<object>>();
        }

        public void AddService<TSource, TDestination>()
        {
            this.dependencyContainer[typeof(TSource)] = typeof(TDestination);
        }

        public void AddService<T>(Func<T> func)
        {
            this.dependencyContainerWithFunc.Add(typeof(T), () => func());
        }

        public T CreateInstace<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        public object CreateInstance(Type type)
        {
            if (this.dependencyContainerWithFunc.ContainsKey(type))
            {
                return this.dependencyContainerWithFunc[type]();
            }

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
