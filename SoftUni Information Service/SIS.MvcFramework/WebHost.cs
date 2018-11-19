using SIS.MvcFramework.Interfaces;
using SIS.WebServer;
using SIS.WebServer.Routing;

namespace SIS.MvcFramework
{
    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            application.ConfigureServices();

            application.Configure(serverRoutingTable);


            var server = new Server(80, serverRoutingTable);
            server.Run();
        }
    }
}
