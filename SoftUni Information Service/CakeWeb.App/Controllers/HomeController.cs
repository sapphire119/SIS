namespace CakeWeb.App.Controllers
{
    using SIS.HTTP.Responses.Interfaces;

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
