namespace CakeWeb.App.Controllers
{
    using CakesWeb.Data;
    using CakesWeb.Services;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class BaseController
    {
        private const string RelativePath = "../../../";

        protected const string InternalDbError = "Invalid request sent to the database";

        protected BaseController()
        {
            this.Db = new CakesDbContext();
            this.CookieService = new UserCookieService();
            this.HashService = new HashService();
        }

        protected CakesDbContext Db { get; }
        protected UserCookieService CookieService { get; }
        protected HashService HashService { get; }

        protected IHttpResponse View(string viewName, IDictionary<string,string> viewBag = null)
        {
            if (viewBag == null) viewBag = new Dictionary<string, string>();
            var allContent = this.GetViewContent(viewName, viewBag);

            return new HtmlResult(allContent, HttpResponseStatusCode.Ok);
        }

        protected IHttpResponse ErrorView(string message,
            HttpResponseStatusCode status = HttpResponseStatusCode.BadRequest)
        {
            var viewBag = new Dictionary<string, string>();
            viewBag.Add("Error", message);
            var allConntent = this.GetViewContent("Error", viewBag);

            return new HtmlResult(allConntent, status);
        }

        protected bool ValidateUrl(string cakeUrl)
        {
            var decodedUrl = WebUtility.UrlDecode(cakeUrl);
            bool result = Uri.TryCreate(decodedUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        private string GetViewContent(string viewName, IDictionary<string, string> viewBag)
        {
            var layoutContent = File.ReadAllText("Views/_Layout.html");

            var controllerName = this.GetType().Name.Replace("Controller", "");

            string content;
            if (viewName == "Error") content 
                    = File.ReadAllText(string.Concat(RelativePath, "Views/Error.html"));
            else content
                    = File.ReadAllText(string.Concat(RelativePath, "Views/", controllerName, "/", viewName, ".html"));
            
            
            foreach (var item in viewBag)
            {
                content = content.Replace("@Model." + item.Key, item.Value);
            }

            var contentResult = layoutContent.Replace("@RenderBody()", content);

            return contentResult;
        }
    }
}
