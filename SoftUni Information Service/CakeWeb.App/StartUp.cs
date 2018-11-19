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
                request => new HomeController { Request=request }.Index();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/Index"] =
                request => new HomeController { Request = request }.Index();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Register"] =
                request => new UsersController { Request = request }.GetRegister();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Login"] =
                request => new UsersController { Request = request }.GetLogin();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Details"] =
                request => new UsersController { Request = request }.GetProfileInfo();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Logout"] =
                request => new UsersController { Request = request }.LogOut();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Cakes/AddCake"] =
                request => new CakesController { Request = request }.GetCakeView();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Cakes/Search"] =
                request => new CakesController { Request = request }.GetSearchView();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Cakes/Details"] =
                request => new CakesController { Request = request }.GetDetailsView();

            //POST
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Register"] =
                request => new UsersController { Request = request }.PostRegister();

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Login"] =
                request => new UsersController { Request = request }.PostLogin();

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Cakes/AddCake"] =
                request => new CakesController { Request = request }.PostCakeView();

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Cakes/Search"] =
                request => new CakesController { Request = request }.PostSearchView();

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Orders/AddToCart"] =
                request => new OrdersController { Request = request }.PostAddToCart();


            Server server = new Server(80, serverRoutingTable);

            server.Run();
        }
    }
}
