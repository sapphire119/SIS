namespace IRunes.App
{
    using IRunes.App.Controllers;
    using IRunes.Data;
    using IRunes.Models;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
    using System;

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
                request => new UsersController().GetLoginView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Register"] =
                request => new UsersController().GetRegisterView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/All"] =
                request => new AlbumsController().GetAllAbumsView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/Details"] =
                request => new AlbumsController().GetDetailsView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Tracks/Create"] =
                request => new TracksController().GetCreateTrackView(request);

            //serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/IndexLoggedin"] =
            //    request => new HomeController().IndexLoggedin(request);

            //
            //
            //
            //Post
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Login"] =
                request => new UsersController().PostLoginView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Register"] =
                request => new UsersController().PostRegisterView(request);

            Server server = new Server(80, serverRoutingTable);

            server.Run();
        }
    }
}
