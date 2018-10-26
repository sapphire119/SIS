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

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = 
                request => new HomeController().Index(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/register"] =
                request => new AccountController().GetRegister(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/register"] =
                request => new AccountController().PostRegister(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/login"] =
                request => new AccountController().GetLogin(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/login"] =
                request => new AccountController().PostLogin(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/logout"] =
                request => new AccountController().LogOut(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/myProfile"] =
                request => new AccountController().GetProfileInfo(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/addCake"] =
                request => new AccountController().GetCake(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/addCake"] =
                request => new AccountController().PostCake(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/search"] =
                request => new AccountController().GetBrowseCakes(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/search"] =
                request => new AccountController().PostBrowseCakes(request);

            Server server = new Server(80, serverRoutingTable);

            server.Run();
        }
    }
}
