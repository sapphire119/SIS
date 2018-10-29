namespace IRunes.App
{
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

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = request => new HomeController().Index();

            Server server = new Server(80, serverRoutingTable);

            server.Run();


        }
    }
}
