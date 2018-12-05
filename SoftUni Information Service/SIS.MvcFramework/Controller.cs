namespace SIS.MvcFramework
{
    using SIS.MvcFramework.Services.Contracts;

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
    using SIS.MvcFramework.RenderEngine.Contracts;
    using SIS.MvcFramework.RenderEngine;
    using SIS.HTTP.Extensions;

    public abstract class Controller
    {
        private const string RelativePath = "../../../";

        protected const string InternalDbError = "Invalid request sent to the database";

        public Controller()
        {
            this.Response = new HttpResponse();
            this.ErrorViewModel = new ErrorViewModel();
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

        public IViewEngine ViewEngine { get; set; }

        public ErrorViewModel ErrorViewModel { get; set; }

        public IUserCookieService CookieService { get; internal set; }

        protected string User
        {
            get
            {
                if (!this.Request.Cookies.ContainsCookie(".auth-cookie"))
                {
                    return null;
                }

                var cookie = this.Request.Cookies.GetCookie(".auth-cookie");
                var username = this.CookieService.GetUserData(cookie.Value);
                return username;
            }
        }

        protected IHttpResponse View(string viewName)
        {
            var allContent = this.GetViewContent(viewName, (object)null);

            return this.Html(allContent);
        }

        protected IHttpResponse View<T>(string viewName, T model = null)
            where T : class
        {
            var allContent = this.GetViewContent(viewName, model);

            return this.Html(allContent);
        }

        private string GetViewContent<T>(string viewName, T model)
        {
            var controllerName = this.GetType().Name.Replace("Controller", "");

            string content;
            if (viewName == "Error") content
                    = System.IO.File.ReadAllText(string.Concat(RelativePath, "Views/Error.html"));
            else content
                    = System.IO.File.ReadAllText(string.Concat(RelativePath, "Views/", controllerName, "/", viewName, ".html"));

            var allContent = this.ViewEngine.GetHtml(viewName, content, model, this.User);

            var layoutFileContent = System.IO.File.ReadAllText("Views/_Layout.html");

            var contentResult = layoutFileContent.Replace("@RenderBody()", allContent);

            var layoutContent = this.ViewEngine.GetHtml("_Layout", contentResult, model, this.User);

            return layoutContent;
        }

        protected bool ValidateUrl(string cakeUrl)
        {
            var decodedUrl = cakeUrl.UrlDecode();
            bool result = Uri.TryCreate(decodedUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        protected IHttpResponse ErrorView(string message,
            HttpResponseStatusCode status = HttpResponseStatusCode.BadRequest)
        {
            this.ErrorViewModel.Error = message;

            var allConntent = this.GetViewContent("Error", this.ErrorViewModel);

            this.Response.Content = Encoding.UTF8.GetBytes(allConntent);
            this.Response.Headers.Add(new HttpHeader("Content-Type", "text/html; charset=utf-8"));
            this.Response.StatusCode = status;

            return this.Response;
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
    }
}
