namespace IRunes.App.Controllers
{
    using IRunes.Data;
    using IRunes.Services;
    using IRunes.Services.Interfaces;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class BaseController
    {
        private const string FileRealtivePath = "../../../";

        public BaseController()
        {
            this.Db = new IRunesDbContext();
            this.UserCookieService = new UserCookieService();
            this.HashService = new HashService();
            this.ViewBag = new Dictionary<string, string>();
        }

        public IRunesDbContext Db { get; }

        public IUserCookieService UserCookieService { get; }

        public IHashService HashService { get; }

        protected IDictionary<string, string> ViewBag { get; set; }

        public IHttpResponse View([CallerMemberName] string action = "")
        {
            var controllerName = this.GetType().Name.Replace("Controller", "");

            var content = File.ReadAllText(FileRealtivePath + "Views/" + controllerName + "/" + action + ".html");

            foreach (var vbkey in this.ViewBag.Keys)
            {
                string dynamicPlaceholder = $"{{{{{vbkey}}}}}";

                if (content.Contains(dynamicPlaceholder))
                {
                    content = content.Replace(dynamicPlaceholder, this.ViewBag[vbkey]);
                }
            }   

            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }

        protected IHttpResponse ErrorView(string message,
            HttpResponseStatusCode status = HttpResponseStatusCode.BadRequest)
        {
            return new HtmlResult(message, status);
        }

        protected bool ValidateUrlFormat(string source)
        {
            var decodedUrl = WebUtility.UrlDecode(source);

            return Uri.TryCreate(decodedUrl, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
