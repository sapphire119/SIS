namespace IRunes.App.Controllers
{
    using IRunes.Data;
    using IRunes.Services;
    using IRunes.Services.Interfaces;

    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;

    using System.IO;

    public class BaseController
    {
        public BaseController()
        {
            this.Db = new IRunesDbContext();
            this.UserCookieService = new UserCookieService();
            this.HashService = new HashService();
        }

        public IRunesDbContext Db { get; }

        public IUserCookieService UserCookieService { get; }

        public IHashService HashService { get; }

        public IHttpResponse View(string path)
        {
            var content = File.ReadAllText("Views/" + path + ".html");

            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }

        public IHttpResponse ErrorView(string message, HttpResponseStatusCode status)
        {
            return new HtmlResult(message, status);
        }
    }
}
