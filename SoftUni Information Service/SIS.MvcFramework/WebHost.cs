namespace SIS.MvcFramework
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;

    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.Interfaces;
    using SIS.MvcFramework.Services;
    using SIS.MvcFramework.Services.Contracts;
    using SIS.WebServer;
    using SIS.WebServer.Results;
    using SIS.WebServer.Routing;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            var dependencyContainer = new ServiceCollection();

            application.ConfigureServices(dependencyContainer);

            AutoRegisterRoutes(serverRoutingTable, application, dependencyContainer);

            application.Configure();


            var server = new Server(80, serverRoutingTable);
            server.Run();
        }

        private static void AutoRegisterRoutes(
            ServerRoutingTable serverRoutingTable, IMvcApplication application, IServiceCollection serviceCollection)
        {
            var controllers = application.GetType().Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract
                && t.IsSubclassOf(typeof(Controller)))
                .ToList();

            foreach (var controller in controllers)
            {
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.CustomAttributes.Any(a => a.AttributeType.IsSubclassOf(typeof(HttpAttribute))))
                    .ToList();

                foreach (var methodInfo in methods)
                {
                    var httpAttributes = methodInfo.GetCustomAttributes(true)
                        .Where(
                        ca => ca.GetType().IsSubclassOf(typeof(HttpAttribute)))
                        .Cast<HttpAttribute>()
                        //.Select(o => (HttpAttribute)o)
                        .ToList();

                    if (httpAttributes == null) continue;

                    foreach (var httpAttribute in httpAttributes)
                    {
                        Console.WriteLine(
                            $"Route registered: {controller.FullName} => {methodInfo.Name} => {httpAttribute.Path}");

                        serverRoutingTable.Add(httpAttribute.RequestMethod, httpAttribute.Path,
                            (request) => ExecuteAction(controller, methodInfo, request, serviceCollection));
                    }

                }
            }
        }

        private static IHttpResponse ExecuteAction(
            Type controllerName, MethodInfo methodInfo, IHttpRequest request, IServiceCollection serviceCollection)
        {
            var controllerInstance = serviceCollection.CreateInstance(controllerName) as Controller;

            if (controllerInstance == null)
            {
                return new TextResult("Controller not found.", HttpResponseStatusCode.InternalServerError);
            }

            controllerInstance.Request = request;
            controllerInstance.CookieService = serviceCollection.CreateInstace<IUserCookieService>();

            var actionParameters = methodInfo.GetParameters();

            var actionParametersObjects = new List<object>();

            foreach (var actionParameter in actionParameters)
            {
                var instance = serviceCollection.CreateInstance(actionParameter.ParameterType);

                var propertires = actionParameter.ParameterType.GetProperties();

                foreach (var propertyInfo in propertires)
                {
                    // TODO: Support IEnumerable 
                    var key = propertyInfo.Name.ToUpper();
                    object value = null;
                    if (request.FormData.Any(x => x.Key.ToUpper() == key))
                    {
                        value = request.FormData.First(x => x.Key.ToUpper() == key).Value.ToString();
                    }
                    else if (request.QueryData.Any(x => x.Key.ToUpper() == key))
                    {
                        value = request.QueryData.First(x => x.Key.ToUpper() == key).Value.ToString();
                    }

                    propertyInfo.SetMethod.Invoke(instance, new object[] { value });
                }

                actionParametersObjects.Add(instance);
            }

            var response = methodInfo
                .Invoke(controllerInstance, actionParametersObjects.ToArray()) 
                as IHttpResponse;

            return response;
        }
    }
}
