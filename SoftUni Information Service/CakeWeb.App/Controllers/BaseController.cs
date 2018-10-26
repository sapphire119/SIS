namespace CakeWeb.App.Controllers
{
    using CakesWeb.Data;
    using CakesWeb.Services;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;
    using System.IO;
    using System.Text;

    public class BaseController
    {
        protected BaseController()
        {
            this.Db = new CakesDbContext();
            this.CookieService = new UserCookieService();
            this.HashService = new HashService();
        }

        protected CakesDbContext Db { get; }
        protected UserCookieService CookieService { get; }
        protected HashService HashService { get; }

        protected IHttpResponse View(string viewName)
        {
            var content = File.ReadAllText("Views/" + viewName + ".html");

            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }

        protected IHttpResponse BadRequest()
        {
            var message = new BadRequestException().Message;
            return new HtmlResult(message, HttpResponseStatusCode.BadRequest);
        }
    }
}
