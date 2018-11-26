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
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

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

            var actionParametersObjects = 
                GetActionParametersObjects(methodInfo, request, serviceCollection);

            var response = methodInfo
                .Invoke(controllerInstance, actionParametersObjects.ToArray())
                as IHttpResponse;

            return response;
        }

        private static List<object> GetActionParametersObjects
            (MethodInfo methodInfo, IHttpRequest request, IServiceCollection serviceCollection)
        {
            var actionParameters = methodInfo.GetParameters();

            var actionParametersObjects = new List<object>();

            foreach (var actionParameter in actionParameters)
            {
                // TODO: Improve this check
                if (actionParameter.ParameterType.IsValueType 
                    || Type.GetTypeCode(actionParameter.ParameterType) == TypeCode.String)
                {
                    var stringValue = GetRequestData(request, actionParameter.Name);
                    var objectParam = TryParseTypeCode(stringValue, actionParameter.ParameterType);
                    actionParametersObjects.Add(objectParam);
                }
                else
                {
                    var instance = serviceCollection.CreateInstance(actionParameter.ParameterType);

                    var propertires = actionParameter.ParameterType.GetProperties();

                    foreach (var propertyInfo in propertires)
                    {
                        // TODO: Support IEnumerable 
                        string stringValue = GetRequestData(request, propertyInfo.Name);

                        object value = TryParseTypeCode(stringValue, propertyInfo.PropertyType);

                        propertyInfo.SetMethod.Invoke(instance, new object[] { value });
                    }

                    actionParametersObjects.Add(instance);
                }
            }

            return actionParametersObjects;
        }

        private static string GetRequestData(IHttpRequest request, string key)
        {
            key = key.ToUpper();
            string stringValue = null;
            if (request.FormData.Any(x => x.Key.ToUpper() == key))
            {
                stringValue = request.FormData.First(x => x.Key.ToUpper() == key).Value.ToString();
            }
            else if (request.QueryData.Any(x => x.Key.ToUpper() == key))
            {
                stringValue = request.QueryData.First(x => x.Key.ToUpper() == key).Value.ToString();
            }

            return stringValue;
        }

        private static object TryParseTypeCode(string stringValue, Type type)
        {
            var typeCode = Type.GetTypeCode(type);

            object value = null;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    if (bool.TryParse(stringValue, out var boolValue)) value = boolValue;
                    break;
                case TypeCode.Byte:
                    if (byte.TryParse(stringValue, out var byteValue)) value = byteValue;
                    break;
                case TypeCode.Char:
                    if (char.TryParse(stringValue, out var charValue)) value = charValue;
                    break;
                case TypeCode.DateTime:
                    if (DateTime.TryParse(stringValue, out var dateTimeValue)) value = dateTimeValue;
                    break;
                case TypeCode.Decimal:
                    if (decimal.TryParse(stringValue, out var decimalValue)) value = decimalValue;
                    break;
                case TypeCode.Double:
                    if (double.TryParse(stringValue, out var doubleValue)) value = doubleValue;
                    break;
                case TypeCode.Int32:
                    if (int.TryParse(stringValue, out var intValue)) value = intValue;
                    break;
                case TypeCode.String:
                    value = stringValue;
                    break;
            }

            return value;
        }
    }
}
