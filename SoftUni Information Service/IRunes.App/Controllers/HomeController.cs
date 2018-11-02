﻿namespace IRunes.App.Controllers
{ 
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;

    using System.Linq;
    using System.Net;
    using System.Text;

    public class HomeController : BaseController
    {
        public IHttpResponse Index(IHttpRequest request)
        {
            if (request.Cookies.ContainsCookie(".auth-cookie"))
            {
                var cookie = request.Cookies.GetCookie(".auth-cookie");

                var usernameOrEmail = this.UserCookieService.GetUserData(cookie.Value);

                this.ViewBag["usernameOrEmail"] = WebUtility.UrlDecode(usernameOrEmail);

                var response = this.View("IndexLoggedin");

                return response;
            }

            return this.View();
        }
    }
}
