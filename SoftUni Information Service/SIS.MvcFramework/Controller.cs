namespace SIS.MvcFramework
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.MvcFramework.Services;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    public class Controller
    {
        private const string RelativePath = "../../../";

        protected const string InternalDbError = "Invalid request sent to the database";

        public Controller()
        {
            this.CookieService = new UserCookieService();
            this.HashService = new HashService();

            this.Response = new HttpResponse();
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }


        protected IUserCookieService CookieService { get; }
        protected IHashService HashService { get; }

        protected IHttpResponse View(string viewName, IDictionary<string, string> viewBag = null)
        {
            if (viewBag == null) viewBag = new Dictionary<string, string>();
            var allContent = this.GetViewContent(viewName, viewBag);

            return this.Html(allContent);
        }

        protected IHttpResponse ErrorView(string message,
            HttpResponseStatusCode status = HttpResponseStatusCode.BadRequest)
        {
            var viewBag = new Dictionary<string, string>();
            viewBag.Add("Error", message);
            var allConntent = this.GetViewContent("Error", viewBag);
            this.Response.StatusCode = status;

            return this.Html(allConntent);
        }

        protected bool ValidateUrl(string cakeUrl)
        {
            var decodedUrl = WebUtility.UrlDecode(cakeUrl);
            bool result = Uri.TryCreate(decodedUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        protected IHttpResponse Redirect(string location)
        {
            this.Response.Headers.Add(new HttpHeader("Location", location));
            this.Response.StatusCode = HttpResponseStatusCode.SeeOther;

            return this.Response;
        }

        protected IHttpResponse File(byte[] content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentDisposition, "inline"));
            this.Response.Content = content;
            this.Response.StatusCode = HttpResponseStatusCode.Ok;

            return this.Response;
        }

        protected IHttpResponse Text(string content)
        {
            this.Response.Headers.Add(new HttpHeader("Content-Type", "text/plain"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
            this.Response.StatusCode = HttpResponseStatusCode.Ok;

            return this.Response;
        }

        protected IHttpResponse Html(string content)
        {
            this.Response.Headers.Add(new HttpHeader("Content-Type", "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
            this.Response.StatusCode = HttpResponseStatusCode.Ok;

            return this.Response;
        }

        private string GetViewContent(string viewName, IDictionary<string, string> viewBag)
        {
            var layoutContent = System.IO.File.ReadAllText("Views/_Layout.html");

            var controllerName = this.GetType().Name.Replace("Controller", "");

            string content;
            if (viewName == "Error") content
                    = System.IO.File.ReadAllText(string.Concat(RelativePath, "Views/Error.html"));
            else content
                    = System.IO.File.ReadAllText(string.Concat(RelativePath, "Views/", controllerName, "/", viewName, ".html"));


            foreach (var item in viewBag)
            {
                content = content.Replace("@Model." + item.Key, item.Value);
            }

            var contentResult = layoutContent.Replace("@RenderBody()", content);

            return contentResult;
        }
    }
}
