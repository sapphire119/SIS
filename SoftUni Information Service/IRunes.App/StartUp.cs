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

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/Create"] =
                request => new AlbumsController().GetCreateView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/Details"] =
                request => new AlbumsController().GetDetailsView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Tracks/Create"] =
                request => new TracksController().GetCreateTrackView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Tracks/Details"] =
                request => new TracksController().GetDetailsTrackView(request);


            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Logout"] =
                request => new UsersController().GetLogout(request);
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

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Albums/Create"] =
                request => new AlbumsController().PostCreateView(request);

            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Tracks/Create"] =
                request => new TracksController().PostCreateTrackView(request);

            Server server = new Server(80, serverRoutingTable);

            server.Run();
        }
    }
}
