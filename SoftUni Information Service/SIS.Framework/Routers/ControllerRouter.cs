namespace SIS.Framework.Routers
{
    using SIS.Framework.Controllers;

    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;

    using SIS.WebServer.Api;

    using System.Collections.Generic;
    using System.Reflection;

    public class ControllerRouter : IHttpHandler
    {
        private Controller GetController(string controllerName, IHttpRequest request)
        {
            return null;
        }

        private MethodInfo GetMethod(string requestMethod, Controller controller, string actionName)
        {
            return null;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            return null;
        }

        private IHttpResponse PrepareResponse(Controller controller, MethodInfo action)
        {
            return null;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
