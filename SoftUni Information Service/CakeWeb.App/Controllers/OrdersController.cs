namespace CakeWeb.App.Controllers
{
    using CakesWeb.Models;
    using CakeWeb.App.ViewModels.Orders;
    using CakeWeb.App.ViewModels.Cakes;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.MvcFramework.Attributes;

    using System.Collections.Generic;
    using System.Linq;
    using SIS.HTTP.Enums;
    using System;

    public class OrdersController : BaseController
    {
        [HttpGet("/Orders/AddToCart")]
        public IHttpResponse AddToCart(int productId)
        {
            if (productId is default(int))
            {
                
            }
            var cakeById = this.CookieService.GetUserCookie(productId.ToString());

            List<string> cakesByIds;
            if (!this.Request.Session.ContainsParamter("cart"))
            {
                cakesByIds = new List<string>();
            }
            else
            {
                cakesByIds = this.Request.Session.GetParameters("cart") as List<string>;
            }

            cakesByIds.Add(cakeById);

            this.Request.Session.AddParamter("cart", cakesByIds);

            var decryptedIds = this.DecryptIdsFromSession(cakesByIds);

            var cakes = this.GetCakesFromDb(decryptedIds);

            var model = new AddToCartViewModel()
            {
                Cakes = cakes,
                TotalPriceOfOrder = cakes.Sum(c => c.CakePrice)
            };

            return this.View("AddToCart", model);
        }

        [HttpGet("Orders/Create")]
        public IHttpResponse Create()
        {
            if (!this.Request.Session.ContainsParamter("cart"))
            {
                return this.ErrorView("Cart is empty!");
            }

            var cakeIds = this.Request.Session.GetParameters("cart") as List<string>;

            var decryptedIds = this.DecryptIdsFromSession(cakeIds);

            this.Request.Session.RemoveSelectedParamter("cart");

            var cakes = this.Db.Products.Where(p => decryptedIds.Any(cakeId => p.Id == cakeId)).ToList();

            var newOrder = new Order();

            try
            {
                foreach (var cake in cakes)
                {
                    this.Db.OrdersProducts.Add(new OrderProduct()
                    {
                        Order = newOrder,
                        Product = cake
                    });
                }

                this.Db.SaveChanges();
            }
            catch (Exception e)
            {
                this.ErrorView(e.ToString(), HttpResponseStatusCode.InternalServerError);
            }


            return this.Redirect("/Orders/All");
        }

        [HttpGet("/Orders/All")]
        public IHttpResponse All()
        {
            if (!this.Request.Cookies.ContainsCookie(".auth-cookie"))
            {
                return this.Redirect("/Users/Register");
            }

            var orders = this.Db.Orders.ToList();

            var allOrdersViewModel = new List<AllViewModel>();

            foreach (var order in orders)
            {
                var model = new AllViewModel(
                    order.Id, 
                    order.DateOfCreation.ToString(@"dd-MM-yyyy"), 
                    order.Products.Sum(p => p.Product.Price));

                allOrdersViewModel.Add(model);
            }

            return this.View("All", allOrdersViewModel.ToArray());
        }

        [HttpGet("/Orders/Details")]
        public IHttpResponse Details(int orderId)
        {
            var currentOrder = this.Db.Orders.FirstOrDefault(o => o.Id == orderId);

            if (currentOrder == null)
            {
                return this.ErrorView($"Order with Id: {orderId} does not exist.");
            }

            var model = new DetailsViewModel(
                currentOrder.Id,
                currentOrder.DateOfCreation.ToString(@"dd-MM-yyyy"));

            var products = new List<CakeViewModel>();
            foreach (var productOrder in currentOrder.Products)
            {
                var product = productOrder.Product;

                var currentModel = new CakeViewModel(product.Id, product.Name.Replace("+", " "), product.Price);

                products.Add(currentModel);
            }

            model.Cakes.AddRange(products);

            return this.View("Details", model);
        }

        private List<int> DecryptIdsFromSession(List<string> cakesByIds)
        {
            return cakesByIds.Select(productId => int.Parse(this.CookieService.GetUserData(productId)))
                .ToList();
        }

        private List<CakeViewModel> GetCakesFromDb(List<int> decryptedIds)
        {
            return this.Db.Products.Where(p => decryptedIds.Any(cakeId => p.Id == cakeId))
                .Select(p => new CakeViewModel(p.Id, p.Name.Replace("+", " "), p.Price)).ToList();
        }
    }
}
