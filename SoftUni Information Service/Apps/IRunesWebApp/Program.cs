
using System;
using System.Collections.Generic;
using IRunesWebApp.Services;
using IRunesWebApp.Services.Contracts;
using Services;
using SIS.Framework;
using SIS.Framework.Routers;
using SIS.Framework.Services;
using SIS.WebServer;

namespace IRunesWebApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dependencyContainer = new DependencyContainer();

            RegisterDependencies(dependencyContainer);

            var handlingContext = new HttpRouteHandlingContext(
                new ControllerRouter(dependencyContainer),
                new ResourceRouter());

            Server server = new Server(80, handlingContext);

            var engine = new MvcEngine();

            engine.Run(server);
        }

        private static void RegisterDependencies(DependencyContainer dependencyContainer)
        {
            dependencyContainer.RegisterDependency<IHashService, HashService>();

            dependencyContainer.RegisterDependency<IUsersService, UsersService>();
        }
    }
}
