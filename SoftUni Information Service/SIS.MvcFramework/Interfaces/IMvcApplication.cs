﻿using SIS.WebServer.Routing;

namespace SIS.MvcFramework.Interfaces
{
    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices();
    }
}