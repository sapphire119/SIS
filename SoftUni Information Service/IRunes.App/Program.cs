namespace IRunes.App
{
    using IRunes.App.Controllers;
    using IRunes.Data;
    using IRunes.Models;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Api;
    using SIS.WebServer.Routing;
    using System;

    public class Program
    {
        public static void Main()
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            ConfigureRouting(serverRoutingTable);

            var handler = new HttpHandler(serverRoutingTable);

            Server server = new Server(80, handler);

            server.Run();
        }

        private static void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        {
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/"] =
                request => new HomeController().Index(request);

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Home/Index"] =
                request => new HomeController().Index(request);

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Users/Login"] =
                request => new UsersController().GetLoginView(request);

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Users/Register"] =
                request => new UsersController().GetRegisterView(request);

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Albums/All"] =
                request => new AlbumsController().GetAllAbumsView(request);

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Albums/Create"] =
                request => new AlbumsController().GetCreateView(request);

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Albums/Details"] =
                request => new AlbumsController().GetDetailsView(request);

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Tracks/Create"] =
                request => new TracksController().GetCreateTrackView(request);

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Tracks/Details"] =
                request => new TracksController().GetDetailsTrackView(request);


            serverRoutingTable.Routes[HttpRequestMethod.GET]["/Users/Logout"] =
                request => new UsersController().GetLogout(request);
            //serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/IndexLoggedin"] =
            //    request => new HomeController().IndexLoggedin(request);

            //
            //
            //
            //Post
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/Users/Login"] =
                request => new UsersController().PostLoginView(request);

            serverRoutingTable.Routes[HttpRequestMethod.POST]["/Users/Register"] =
                request => new UsersController().PostRegisterView(request);

            serverRoutingTable.Routes[HttpRequestMethod.POST]["/Albums/Create"] =
                request => new AlbumsController().PostCreateView(request);

            serverRoutingTable.Routes[HttpRequestMethod.POST]["/Tracks/Create"] =
                request => new TracksController().PostCreateTrackView(request);
        }
    }
}
