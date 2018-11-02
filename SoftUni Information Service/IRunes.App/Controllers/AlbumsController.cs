namespace IRunes.App.Controllers
{
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;
    using System;
    using System.Linq;
    using System.Text;

    public class AlbumsController : BaseController
    {
        private const string EmptyAlbumList = "There are currently no albums.";
        private const string NoTracksInAlbumMessage = "There are currently no tracks in this Album.";

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
                    this.ViewBag["albumName"] = album.Name;

                    var tempView = Encoding.UTF8.GetString(this.View("AlbumTemplate").Content);

                    result += string.Concat(tempView, Environment.NewLine);
                }

                this.ViewBag["albumsFromDb"] = result.Trim();
            }

            var response = this.View("All");

            return response;
        }

        public IHttpResponse GetDetailsView(IHttpRequest request)
        {
            var albumId = Guid.Parse(request.QueryData["id"].ToString());

            var currentAlbum = this.Db.Albums.SingleOrDefault(a => a.Id == albumId);

            this.ViewBag["albumCover"] = $"<img src=\"{currentAlbum.Cover}\" alt=\"{currentAlbum.Name}\" />";
            this.ViewBag["albumName"] = currentAlbum.Name;
            this.ViewBag["albumPrice"] = $"${currentAlbum.Price.ToString()}";
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
                    this.ViewBag["trackName"] = currentTracks[i].Name;

                    var trackHtml = Encoding.UTF8.GetString(this.View("TrackTemplate").Content);

                    trackResult += string.Concat(trackHtml, Environment.NewLine);
                }

                this.ViewBag["tracksOrdered"] = trackResult.Trim();
            }

            return this.View("Details");
        }
    }
}
