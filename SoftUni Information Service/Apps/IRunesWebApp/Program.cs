﻿
using IRunesWebApp.Controllers;
using SIS.Framework;
using SIS.Framework.Routers;
using SIS.HTTP.Enums;
using SIS.WebServer;
using SIS.WebServer.Api;
using SIS.WebServer.Results;
using SIS.WebServer.Routing;
        
namespace IRunesWebApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var handler = new ControllerRouter();
            Server server = new Server(80, handler);
            var engine = new MvcEngine();
            engine.Run(server);
            //MvcEngine.Run(server);
        }

        private static void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        {
            // GET
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/home/index"] =
                request => new RedirectResult("/");
            //serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] =
            //    request => new HomeController().Index(request);
            //serverRoutingTable.Routes[HttpRequestMethod.Get]["/users/login"] =
            //    request => new UsersController().Login(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/users/register"] =
                request => new UsersController().Register(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/albums/all"] =
                request => new AlbumsController().All(request);


            // POST
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/users/login"] =
                request => new UsersController().PostLogin(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/users/register"] =
                request => new UsersController().PostRegister(request);
        }
    }
}
