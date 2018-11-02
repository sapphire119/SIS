namespace IRunes.App.Controllers
{
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;

    using System;
    using System.Linq;
    using System.Text;

    public class TracksController : BaseController
    {
        public IHttpResponse GetCreateTrackView(IHttpRequest request)
        {
            var albumId = Guid.Parse(request.QueryData["albumId"].ToString());

            var currentAlbum = this.Db.Albums.SingleOrDefault(a => a.Id == albumId);

            this.ViewBag["albumId"] = currentAlbum.Id.ToString();

            var response =  this.View("Create");

            return response;
        }
    }
}
