namespace IRunes.App.Controllers
{
    using IRunes.Models;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.HTTP.Sessions;
    using SIS.WebServer.Results;
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    public class AlbumsController : BaseController
    {
        public IHttpResponse GetAllAbumsView(IHttpRequest request)
        {
            var response = this.View("All");

            var stringContent = Encoding.UTF8.GetString(response.Content);



            stringContent = stringContent.Replace("{{albumsFromDb}}", string.Empty);

            return null;
        }
    }
}
