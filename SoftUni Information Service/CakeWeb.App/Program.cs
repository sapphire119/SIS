namespace CakeWeb
{
    using CakeWeb.App;
    using CakeWeb.App.Controllers;
    using SIS.HTTP.Enums;
    using SIS.MvcFramework;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class Program
    {
        public static void Main()
        {
            //IoC == DI (Inversion of Control == Dependency Injection)
            //IoCC == Inversion of Control Container
            WebHost.Start(new StartUp());
        }
    }
}
