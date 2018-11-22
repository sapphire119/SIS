namespace IRunes.App.Controllers
{
    using IRunes.App.Extensions;
    using IRunes.Models;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;

    public class TracksController : BaseController
    {
        private const string InvalidTrackPriceFormat = "Invalid Track Price Format!";
        private const string InvalidUrlTrackFormat = "Given Url Track Is Not In Supported Format!";
        private const string TrackAlreadyExistsInAlbum = "Track with given name already exists!";

        public IHttpResponse GetCreateTrackView(IHttpRequest request)
        {
            var albumId = Guid.Parse(request.QueryData["albumId"].ToString());

            var currentAlbum = this.Db.Albums.SingleOrDefault(a => a.Id == albumId);

            this.ViewBag["albumId"] = currentAlbum.Id.ToString();

            var response =  this.View("Create").ApplyLayout(request);

            return response;
        }

        public IHttpResponse PostCreateTrackView(IHttpRequest request)
        {
            var albumId = Guid.Parse(request.QueryData["albumId"].ToString());

            var currentAlbum = this.Db.Albums.SingleOrDefault(a => a.Id == albumId);

            if (currentAlbum == null)
            {
                return new RedirectResult("/Albums/Create");
            }

            var trackName = request.FormData["trackName"].ToString();

            if (currentAlbum.Tracks.Any(t => t.Name == trackName))
            {
                return this.ErrorView(TrackAlreadyExistsInAlbum);
            }

            var trackLink = request.FormData["trackLink"].ToString();
            var isValidPrice = decimal.TryParse(request.FormData["trackPrice"].ToString(), out var trackPrice);

            if (!isValidPrice)
            {
                return this.ErrorView(InvalidTrackPriceFormat);
            }

            var isValidUrl = this.ValidateUrlFormat(trackLink);

            if (!isValidUrl)
            {
                return this.ErrorView(InvalidUrlTrackFormat);
            }

            var track = new Track(trackName, trackLink, trackPrice, albumId);

            this.Db.Tracks.Add(track);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return this.ErrorView("Coudn't save track on server!", HttpResponseStatusCode.InternalServerError);
            }

            var response = new RedirectResult($"/Albums/Details?id={albumId}");

            return response;
        }
        
        public IHttpResponse GetDetailsTrackView(IHttpRequest request)
        {
            var trackId = Guid.Parse(request.QueryData["trackId"].ToString());

            var albumId = Guid.Parse(request.QueryData["albumId"].ToString());

            var currentTrack = this.Db.Tracks.SingleOrDefault(t => t.Id == trackId && t.AlbumId == albumId);

            var decodedTrackLink = WebUtility.UrlDecode(currentTrack.Link);

            var linkTokens = decodedTrackLink.Split("?");

            var trackLinkTokens = linkTokens[1].Split("=", 2);

            var trackLinkId = trackLinkTokens[1];

            this.ViewBag["trackLink"] = trackLinkId;
            this.ViewBag["trackName"] = currentTrack.Name.Replace("+", " ");
            this.ViewBag["trackPrice"] = $"${currentTrack.Price.ToString()}";

            this.ViewBag["albumId"] = currentTrack.AlbumId.ToString();

            var response = this.View("Details").ApplyLayout(request);

            return response;
        }
    }
}
