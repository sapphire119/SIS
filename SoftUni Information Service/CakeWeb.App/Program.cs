namespace CakeWeb
{
    using CakeWeb.App;
    using CakeWeb.App.Controllers;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class Program
    {
        public static void Main()
        {
            WebHost.Start(new StartUp());
        }
    }
}
