namespace SIS.Framework
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Interfaces;

    using SIS.WebServer.Api;
    using SIS.WebServer.Results;
    using SIS.WebServer.Routing;

    using System.IO;

    public class HttpHandler : IHttpHandler
    {
        private readonly ServerRoutingTable serverRoutingTable;

        public HttpHandler(ServerRoutingTable serverRoutingTable)
        {
            this.serverRoutingTable = serverRoutingTable;
        }

        public IHttpResponse Handle(IHttpRequest httpRequest)
        {
            if (!this.serverRoutingTable.Routes.ContainsKey(httpRequest.RequestMethod)
                || !this.serverRoutingTable.Routes[httpRequest.RequestMethod].ContainsKey(httpRequest.Path))
            {
                return this.ReturnIfResource(httpRequest.Path);
            }

            return this.serverRoutingTable.Routes[httpRequest.RequestMethod][httpRequest.Path].Invoke(httpRequest);
        }

        private IHttpResponse ReturnIfResource(string path)
        {
            var extension = path.Substring(path.LastIndexOf('.') + 1);

            var root = string.Concat(Directory.GetCurrentDirectory(), "/Resources/", $"{extension}/", path.Substring(path.LastIndexOf('/') + 1));

            if (File.Exists(root))
            {
                var allContent = File.ReadAllBytes(root);

                return new InlineResourceResult(allContent, HttpResponseStatusCode.Ok);
            }

            return new HttpResponse(HttpResponseStatusCode.NotFound);
        }
    }
}
