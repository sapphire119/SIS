namespace SIS.Demo
{
    using System;
    using System.Linq;
    using System.Text;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using SIS.WebServer;
    using SIS.WebServer.Results;
    using SIS.WebServer.Routing;

    public class Launcher
    {
        public static void Main()
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = request => new HomeController().Index();

            Server server = new Server(8000, serverRoutingTable);

            server.Run();



            //StringBuilder sb = new StringBuilder();
            //string input;
            //while ((input = Console.ReadLine()) != "End")
            //{
            //    sb.AppendLine(input);
            //}

            //var httpRequest = new HttpRequest(sb.ToString().Trim());
        }
    }
}
