﻿namespace CakeWeb.App.Controllers
{
    using CakesWeb.Models;
    using CakeWeb.App.ViewModels.Cakes;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.Loggers.Contracts;
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

        private readonly ILogger logger;

        public CakesController(ILogger logger)
        {
            this.logger = logger;
        }

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
        public IHttpResponse PostCakeView(PostCakeViewInputModel model, decimal productPrice)
        {
            //var nameOfProduct = this.Request.FormData["productName"].ToString();
            //var priceOfProduct = this.Request.FormData["productPrice"].ToString();
            //var productUrl = this.Request.FormData["pictureUrl"].ToString();

            //var isItValidPrice = decimal.TryParse(model.ProductPrice, out var parsedPriceOfProduct);
            //if (!isItValidPrice)
            //{
            //    return this.ErrorView(InvalidCakePrice);
            //}

            this.logger.Log(productPrice.ToString());

            if (!ValidateUrl(model.PictureUrl))
            {
                return this.ErrorView(string.Format(PictureOfProductNotValid, model.PictureUrl));
            }

            var product = new Product(model.ProductName, model.ProductPrice/*parsedPriceOfProduct*/, model.PictureUrl);

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
        public IHttpResponse GetSearchView(GetSearchViewInputModel model)
        {
            if (!this.Request.Cookies.ContainsCookie(".auth-cookie")) return this.Redirect("/Users/Register");

            //var viewBag = new Dictionary<string, string>();

            //if (this.Request.Session.ContainsParamter("cakes"))
            //{
            //    viewBag["cakesList"] = EmptyCakesListMessage;
            //}
            if(this.Request.Session.ContainsParamter("cakes"))
            {
                var paramterValue = (GetSearchViewInputModel)this.Request.Session.GetParameters("cakes");

                model.CakesList = paramterValue.CakesList;
                //viewBag["cakesList"] = paramterValue;
            }

            var response = this.View("Search", model);

            return response;
        }

        [HttpPost("/Cakes/Search")]
        public IHttpResponse PostSearchView(CakeViewModel model)
        {
            //var productNameToFind = this.Request.FormData["searchField"].ToString();

            var product = this.Db.Products.FirstOrDefault(c => c.Name == model.CakeName.Trim());
            if (product == null)
            {
                return this.Redirect("/Cakes/Search");
            }

            //var viewBag = new Dictionary<string, string>();
            model.CakeId = product.Id;
            model.CakeName = product.Name.Replace('+', ' ');
            model.CakePrice = product.Price;
            //viewBag["cakeId"] = product.Id.ToString();
            //viewBag["cakeName"] = product.Name.Replace('+', ' ');
            //viewBag["cakePrice"] = product.Price.ToString();

            if (!this.Request.Session.ContainsParamter("cakes"))
            {
                var cakes = new GetSearchViewInputModel();
                cakes.CakesList.Add(model);
                this.Request.Session.AddParamter("cakes", cakes);
            }
            else
            {
                var cakes = (GetSearchViewInputModel)this.Request.Session.GetParameters("cakes");

                cakes.CakesList.Add(model);
                ;
                this.Request.Session.ClearParameters();

                this.Request.Session.AddParamter("cakes", cakes);
            }

            var response = this.Redirect("/Cakes/Search");

            return response;
        }

        [HttpGet("/Cakes/Details")]
        public IHttpResponse GetDetailsView(GetDetailsViewInputModel model)
        {
            //var cakeId = int.Parse(model.Id);

            var currentCake = this.Db.Products.FirstOrDefault(c => c.Id == model.Id);
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