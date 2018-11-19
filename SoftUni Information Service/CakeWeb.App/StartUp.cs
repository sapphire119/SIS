namespace CakeWeb.App
{
    using CakeWeb.App.Controllers;

    using SIS.HTTP.Enums;
    using SIS.MvcFramework.Interfaces;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class StartUp : IMvcApplication
    {
        public void Configure(ServerRoutingTable routing)
        {
            //GET
            routing.Routes[HttpRequestMethod.Get]["/"] =
                request => new HomeController { Request = request }.Index();

            routing.Routes[HttpRequestMethod.Get]["/Home/Index"] =
                request => new HomeController { Request = request }.Index();

            routing.Routes[HttpRequestMethod.Get]["/Users/Register"] =
                request => new UsersController { Request = request }.GetRegister();

            routing.Routes[HttpRequestMethod.Get]["/Users/Login"] =
                request => new UsersController { Request = request }.GetLogin();

            routing.Routes[HttpRequestMethod.Get]["/Users/Details"] =
                request => new UsersController { Request = request }.GetProfileInfo();

            routing.Routes[HttpRequestMethod.Get]["/Users/Logout"] =
                request => new UsersController { Request = request }.LogOut();

            routing.Routes[HttpRequestMethod.Get]["/Cakes/AddCake"] =
                request => new CakesController { Request = request }.GetCakeView();

            routing.Routes[HttpRequestMethod.Get]["/Cakes/Search"] =
                request => new CakesController { Request = request }.GetSearchView();

            routing.Routes[HttpRequestMethod.Get]["/Cakes/Details"] =
                request => new CakesController { Request = request }.GetDetailsView();

            //POST
            routing.Routes[HttpRequestMethod.Post]["/Users/Register"] =
                request => new UsersController { Request = request }.PostRegister();

            routing.Routes[HttpRequestMethod.Post]["/Users/Login"] =
                request => new UsersController { Request = request }.PostLogin();

            routing.Routes[HttpRequestMethod.Post]["/Cakes/AddCake"] =
                request => new CakesController { Request = request }.PostCakeView();

            routing.Routes[HttpRequestMethod.Post]["/Cakes/Search"] =
                request => new CakesController { Request = request }.PostSearchView();

            routing.Routes[HttpRequestMethod.Post]["/Orders/AddToCart"] =
                request => new OrdersController { Request = request }.PostAddToCart();
    }

        public void ConfigureServices()
        {
            // TODO: Implement IoC/DI Container (Inversion of Control)
            return;
            //throw new System.NotImplementedException();
        }
    }
}
