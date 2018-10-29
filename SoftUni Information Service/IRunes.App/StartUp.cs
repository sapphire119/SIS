namespace IRunes.App
{
    using IRunes.App.Controllers;
    using IRunes.Data;
    using IRunes.Models;
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

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/Index"] = 
                request => new HomeController().Index(request);


            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Login"] =
                request => new AccountController().GetLoginView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Login"] =
                request => new AccountController().PostLoginView(request);

            Server server = new Server(80, serverRoutingTable);

            server.Run();
        }
    }
}
