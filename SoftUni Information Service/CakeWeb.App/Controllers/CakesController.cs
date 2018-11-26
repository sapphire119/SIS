namespace CakeWeb.App.Controllers
{
    using CakesWeb.Models;
    using CakeWeb.App.ViewModels.Cakes;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes;
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

        [HttpGet("/Cakes/AddCake")]
        public IHttpResponse GetCakeView()
        {
            if (!this.Request.Cookies.ContainsCookie(".auth-cookie"))
            {
                return this.Redirect("/Users/Register");
            }

            return this.View("AddCake");
        }

        [HttpPost("/Cakes/AddCake")]
        public IHttpResponse PostCakeView(PostCakeViewInputModel model)
        {
            //var nameOfProduct = this.Request.FormData["productName"].ToString();
            //var priceOfProduct = this.Request.FormData["productPrice"].ToString();
            //var productUrl = this.Request.FormData["pictureUrl"].ToString();

            var isItValidPrice = decimal.TryParse(model.ProductPrice, out var parsedPriceOfProduct);
            if (!isItValidPrice)
            {
                return this.ErrorView(InvalidCakePrice);
            }

            if (!ValidateUrl(model.PictureUrl))
            {
                return this.ErrorView(string.Format(PictureOfProductNotValid, model.PictureUrl));
            }

            var product = new Product(model.ProductName, parsedPriceOfProduct, model.PictureUrl);

            this.Db.Products.Add(product);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return this.ErrorView(InternalDbError);
            }

            var response = this.Redirect("/");
            return response;
        }

        [HttpGet("/Cakes/Search")]
        public IHttpResponse GetSearchView()
        {
            if (!this.Request.Cookies.ContainsCookie(".auth-cookie")) return this.Redirect("/Users/Register");

            var viewBag = new Dictionary<string, string>();

            if (!this.Request.Session.ContainsParamter("cakes"))
            {
                viewBag["cakesList"] = EmptyCakesListMessage;
            }
            else
            {
                var paramterValue = this.Request.Session.GetParameters("cakes").ToString();

                viewBag["cakesList"] = paramterValue;
            }

            var response = this.View("Search", viewBag);

            return response;
        }

        [HttpPost("/Cakes/Search")]
        public IHttpResponse PostSearchView()
        {
            var productNameToFind = this.Request.FormData["searchField"].ToString();

            var product = this.Db.Products.FirstOrDefault(c => c.Name == productNameToFind);
            if (product == null)
            {
                return this.Redirect("/Cakes/Search");
            }

            var viewBag = new Dictionary<string, string>();

            viewBag["cakeId"] = product.Id.ToString();
            viewBag["cakeName"] = product.Name.Replace('+', ' ');
            viewBag["cakePrice"] = product.Price.ToString();

            var tempView = this.View("CakesTemplate", viewBag);

            if (!this.Request.Session.ContainsParamter("cakes"))
            {
                this.Request.Session.AddParamter("cakes", Encoding.UTF8.GetString(tempView.Content));
            }
            else
            {
                var currentParam = this.Request.Session.GetParameters("cakes").ToString();
                currentParam += string.Concat(Environment.NewLine, Encoding.UTF8.GetString(tempView.Content));

                this.Request.Session.ClearParameters();

                this.Request.Session.AddParamter("cakes", currentParam);
            }

            var response = this.Redirect("/Cakes/Search");

            return response;
        }

        [HttpGet("/Cakes/Details")]
        public IHttpResponse GetDetailsView(GetDetailsViewInputModel model)
        {
            var cakeId = int.Parse(model.Id);

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
