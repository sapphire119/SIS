namespace SIS.Demo
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;

    public class HomeController 
    {
        public IHttpResponse Index()
        {
            string content = "<h1>Hello World!</h1>";

            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }
    }
}
