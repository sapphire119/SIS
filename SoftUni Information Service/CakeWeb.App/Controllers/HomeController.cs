namespace CakeWeb.App.Controllers
{
    using SIS.HTTP.Responses.Interfaces;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes;

    public class HomeController : BaseController
    {
        [HttpGet("/")]
        [HttpGet("/Home/Index")]
        public IHttpResponse Index()
        {
            return this.View("Index");
        }
    }
}
