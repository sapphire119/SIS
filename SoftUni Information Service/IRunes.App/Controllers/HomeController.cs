namespace IRunes.App.Controllers
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

                var response = this.View("IndexLoggedin");

                var contentString = Encoding.UTF8.GetString(response.Content);

                contentString = contentString.Replace("{{usernameOrEmail}}", WebUtility.UrlDecode(usernameOrEmail));

                response.Content = Encoding.UTF8.GetBytes(contentString);

                return response;
            }

            return this.View();
        }
    }
}
