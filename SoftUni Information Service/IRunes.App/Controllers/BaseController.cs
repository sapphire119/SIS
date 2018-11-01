namespace IRunes.App.Controllers
{
    using IRunes.Data;
    using IRunes.Services;
    using IRunes.Services.Interfaces;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;

    using System.IO;
    using System.Runtime.CompilerServices;

    public class BaseController
    {
        private const string FileRealtivePath = "../../../";

        public BaseController()
        {
            this.Db = new IRunesDbContext();
            this.UserCookieService = new UserCookieService();
            this.HashService = new HashService();
        }

        public IRunesDbContext Db { get; }

        public IUserCookieService UserCookieService { get; }

        public IHashService HashService { get; }

        public IHttpResponse View([CallerMemberName] string action = "")
        {
            var controllerName = this.GetType().Name.Replace("Controller", "");

            var content = File.ReadAllText(FileRealtivePath + "Views/" + controllerName + "/" + action + ".html");

            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }

        public IHttpResponse ErrorView(string message, 
            HttpResponseStatusCode status = HttpResponseStatusCode.BadRequest)
        {
            return new HtmlResult(message, status);
        }
    }
}
