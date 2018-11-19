namespace CakeWeb.App.Controllers
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;

    public class HomeController : BaseController
    {
        public IHttpResponse Index()
        {
            if (this.Request.Cookies.ContainsCookie(".auth-cookie"))
            {
                return this.View("LoggedIn");
            }
            return this.View("Index");
        }
    }
}
