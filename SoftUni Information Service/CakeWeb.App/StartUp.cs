namespace CakeWeb
{
    using CakeWeb.App.Controllers;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class StartUp
    {
        public static void Main()
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            //GET
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = 
                request => new HomeController().Index(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/Index"] =
                request => new HomeController().Index(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Register"] =
                request => new UsersController().GetRegister(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Login"] =
                request => new UsersController().GetLogin(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Details"] =
                request => new UsersController().GetProfileInfo(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Logout"] =
                request => new UsersController().LogOut(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Cakes/AddCake"] =
                request => new CakesController().GetCakeView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Cakes/Search"] =
                request => new CakesController().GetSearchView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Cakes/Details"] =
                request => new CakesController().GetDetailsView(request);

            //POST
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Register"] =
                request => new UsersController().PostRegister(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Login"] =
                request => new UsersController().PostLogin(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Cakes/AddCake"] =
                request => new CakesController().PostCakeView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Cakes/Search"] =
                request => new CakesController().PostSearchView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Orders/AddToCart"] =
                request => new OrdersController().PostAddToCart(request);


            Server server = new Server(80, serverRoutingTable);

            server.Run();
        }
    }
}
