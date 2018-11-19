namespace CakeWeb.App.Controllers
{
    using CakesWeb.Data;
    using CakesWeb.Services;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.MvcFramework;
    using SIS.WebServer.Results;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class BaseController : Controller
    {
        protected BaseController()
        {
            this.Db = new CakesDbContext();
        }

        protected CakesDbContext Db { get; }
        
    }
}
