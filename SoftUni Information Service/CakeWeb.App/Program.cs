namespace CakeWeb
{
    using CakeWeb.App;
    using CakeWeb.App.Controllers;
    using SIS.HTTP.Enums;
    using SIS.MvcFramework;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Program
    {
        public static void Main()
        { 
            //ms.Seek(0, SeekOrigin.Begin);
            //Assembly assembly = Assembly.Load(ms.ToArray());

            //IoC == DI (Inversion of Control == Dependency Injection)
            //IoCC == Inversion of Control Container
            WebHost.Start(new StartUp());
        }
    }
}
