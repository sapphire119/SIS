using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Intefaces;
using SIS.HTTP.Responses;
using SIS.HTTP.Responses.Interfaces;
using SIS.MvcFramework.Attributes;
using SIS.MvcFramework.Interfaces;
using SIS.WebServer;
using SIS.WebServer.Results;
using SIS.WebServer.Routing;
using System;
using System.Linq;
using System.Reflection;

namespace SIS.MvcFramework
{
    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            application.ConfigureServices();

            AutoRegisterRoutes(serverRoutingTable, application);

            application.Configure();


            var server = new Server(80, serverRoutingTable);
            server.Run();
        }

        private static void AutoRegisterRoutes(ServerRoutingTable serverRoutingTable, IMvcApplication application)
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
                        .Select(o => (HttpAttribute)o)
                        .ToList();

                    if (httpAttributes == null) continue;

                    foreach (var httpAttribute in httpAttributes)
                    {
                        Console.WriteLine(
                            $"Route registered: {controller.FullName} => {methodInfo.Name} => {httpAttribute.Path}");

                        serverRoutingTable.Add(httpAttribute.RequestMethod, httpAttribute.Path,
                            (request) => ExecuteAction(controller, methodInfo, request));
                    }

                }
            }
        }

        private static IHttpResponse ExecuteAction(Type controllerName, MethodInfo actionName, IHttpRequest request)
        {
            var controllerInstance = Activator.CreateInstance(controllerName) as Controller;

            if (controllerInstance == null)
            {
                return new TextResult("Controller not found.", HttpResponseStatusCode.InternalServerError);
            }

            controllerInstance.Request = request;

            var response = actionName.Invoke(controllerInstance, new object[] { }) as IHttpResponse;

            return response;
        }
    }
}
