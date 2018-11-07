namespace CakeWeb.App.Controllers
{
    using CakesWeb.Models;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    public class CakesController : BaseController
    {
        private const string InvalidCakePrice = "Price of cake must be only valid numbers!";

        private const string PictureOfProductNotValid = "{0} /r/n This is not a valid Url!";

        private const string ProductNotFound = "Product with name: {0} not found!";

        private const string EmptyCakesListMessage = "Searched and found cakes will be displayed here.";

        public IHttpResponse GetCakeView(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth-cookie"))
            {
                return new RedirectResult("/Users/Register");
            }

            return this.View("AddCake");
        }

        public IHttpResponse PostCakeView(IHttpRequest request)
        {
            var nameOfProduct = request.FormData["productName"].ToString();
            var priceOfProduct = request.FormData["productPrice"].ToString();
            var productUrl = request.FormData["pictureUrl"].ToString();

            var isItValidPrice = decimal.TryParse(priceOfProduct, out var parsedPriceOfProduct);
            if (!isItValidPrice)
            {
                return this.ErrorView(InvalidCakePrice);
            }

            if (!ValidateUrl(productUrl))
            {
                return this.ErrorView(string.Format(PictureOfProductNotValid, productUrl));
            }

            var product = new Product(nameOfProduct, parsedPriceOfProduct, productUrl);

            this.Db.Products.Add(product);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return this.ErrorView(InternalDbError);
            }

            var response = new RedirectResult("/");
            return response;
        }

        public IHttpResponse GetSearchView(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth-cookie")) return new RedirectResult("/Users/Register");

            var viewBag = new Dictionary<string, string>();

            if (!request.Session.ContainsParamter("cakes"))
            {
                viewBag["cakesList"] = EmptyCakesListMessage;
            }
            else
            {
                var paramterValue = request.Session.GetParameters("cakes").ToString();

                viewBag["cakesList"] = paramterValue;
            }

            var response = this.View("Search", viewBag);

            return response;
        }

        public IHttpResponse PostSearchView(IHttpRequest request)
        {
            var productNameToFind = request.FormData["searchField"].ToString();

            var product = this.Db.Products.FirstOrDefault(c => c.Name == productNameToFind);
            if (product == null)
            {
                return new RedirectResult("/Cakes/Search");
            }

            var viewBag = new Dictionary<string, string>();

            viewBag["cakeId"] = product.Id.ToString();
            viewBag["cakeName"] = product.Name.Replace('+', ' ');
            viewBag["cakePrice"] = product.Price.ToString();

            var tempView = this.View("CakesTemplate", viewBag);

            if (!request.Session.ContainsParamter("cakes"))
            {
                request.Session.AddParamter("cakes", Encoding.UTF8.GetString(tempView.Content));
            }
            else
            {
                var currentParam = request.Session.GetParameters("cakes").ToString();
                currentParam += string.Concat(Environment.NewLine, Encoding.UTF8.GetString(tempView.Content));

                request.Session.ClearParameters();

                request.Session.AddParamter("cakes", currentParam);
            }

            var response = new RedirectResult("/Cakes/Search");

            return response;
        }

        public IHttpResponse GetDetailsView(IHttpRequest request)
        {
            var cakeId = int.Parse(request.QueryData["id"].ToString());

            var currentCake = this.Db.Products.FirstOrDefault(c => c.Id == cakeId);
            if (currentCake == null)
            {
                return this.ErrorView("Cake not Found.");
            }

            var viewBag = new Dictionary<string, string>();

            viewBag["productName"] = currentCake.Name;
            viewBag["productPrice"] = currentCake.Price.ToString();
            viewBag["productLink"] = WebUtility.UrlDecode(currentCake.ImageUrl);


            var response = this.View("Details", viewBag);

            return response;
        }
    }
}
