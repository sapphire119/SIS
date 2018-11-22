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

    public class AlbumsController : BaseController
    {
        private const string EmptyAlbumList = "There are currently no albums.";
        private const string NoTracksInAlbumMessage = "There are currently no tracks in this Album.";
        private const string InvalidPriceFormat = "Price should be a number!";
        private const string InvalidUrlFormat = "Invalid Url!";
        private const string AlbumAlreadyExists = "Album with same name already exists!";

        public IHttpResponse GetAllAbumsView(IHttpRequest request)
        {
            var albums = this.Db.Albums.ToList();

            if (!albums.Any())
            {
                this.ViewBag["albumsFromDb"] = EmptyAlbumList;
            }
            else
            {
                var result = string.Empty;
                foreach (var album in albums)
                {
                    this.ViewBag["albumId"] = album.Id.ToString();
                    this.ViewBag["albumName"] = album.Name.Replace("+", " ");

                    var tempView = Encoding.UTF8.GetString(this.View("AlbumTemplate").Content);

                    result += string.Concat(tempView, Environment.NewLine);
                }

                this.ViewBag["albumsFromDb"] = result.Trim();
            }

            var response = this.View("All").ApplyLayout(request);

            return response;
        }

        public IHttpResponse GetCreateView(IHttpRequest request) => this.View("Create").ApplyLayout(request);

        public IHttpResponse PostCreateView(IHttpRequest request)
        {
            var albumName = request.FormData["albumName"].ToString().Trim();
            var albumCover = request.FormData["albumCover"].ToString();
            var isItAValidPrice = decimal.TryParse(request.FormData["albumPrice"].ToString(), out var albumPrice);

            var isValidUrl = this.ValidateUrlFormat(albumCover);

            if (!isValidUrl)
            {
                return this.ErrorView(InvalidUrlFormat);
            }

            if (!isItAValidPrice)
            {
                return this.ErrorView(InvalidPriceFormat);
            }

            var currentAlbum = this.Db.Albums.SingleOrDefault(a => a.Name == albumName);
            if (currentAlbum != null)
            {
                return this.ErrorView(AlbumAlreadyExists);
            }

            currentAlbum = new Album(albumName, albumCover, albumPrice);

            this.Db.Albums.Add(currentAlbum);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return this.ErrorView("Couldn't save Album on Server", HttpResponseStatusCode.InternalServerError);
            }

            var response = new RedirectResult("/Albums/All");

            return response;
        }

        public IHttpResponse GetDetailsView(IHttpRequest request)
        {
            var albumId = Guid.Parse(request.QueryData["id"].ToString());

            var currentAlbum = this.Db.Albums.SingleOrDefault(a => a.Id == albumId);

            this.ViewBag["albumName"] = currentAlbum.Name.Replace("+", " ");
            this.ViewBag["albumPrice"] = $"${currentAlbum.Price.ToString()}";
            this.ViewBag["albumCover"] = WebUtility.UrlDecode(currentAlbum.Cover);
            this.ViewBag["albumId"] = currentAlbum.Id.ToString();

            var currentTracks = currentAlbum.Tracks.ToList();

            if (!currentTracks.Any())
            {
                this.ViewBag["tracksOrdered"] = NoTracksInAlbumMessage;
            }
            else
            {
                var trackResult = string.Empty;
                for (int i = 0; i < currentTracks.Count; i++)
                {
                    this.ViewBag["almbumSeq"] = (i + 1).ToString();
                    this.ViewBag["trackId"] = currentTracks[i].Id.ToString();
                    this.ViewBag["trackName"] = currentTracks[i].Name.Replace("+", " ");

                    var trackHtml = Encoding.UTF8.GetString(this.View("TrackTemplate").Content);

                    trackResult += string.Concat(trackHtml, Environment.NewLine);
                }

                this.ViewBag["tracksOrdered"] = trackResult.Trim();
            }

            return this.View("Details").ApplyLayout(request);
        }
    }
}
