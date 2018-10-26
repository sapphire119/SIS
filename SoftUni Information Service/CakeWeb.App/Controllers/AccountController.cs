namespace CakeWeb.App.Controllers
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.WebServer.Results;

    using System.Text.RegularExpressions;
    using System.Linq;
    using System;
    using CakesWeb.Models;
    using SIS.HTTP.Cookies;
    using CakesWeb.Services;
    using System.Text;
    using System.Net;

    public class AccountController : BaseController
    {
        private const string PasswordsDontMatchMessage = "Passwords do not match";
        private const string EmptyPasswordOrLengthNotEnough = "Password is empty or length is below 6 characters";
        private const string UserAlreadyExists = "User already exists.";
        private const string InvalidUsernameOrPassword = "No user exists with given username or password";
        private const string UserNotLoggedIn = "User not logged in!";
        private const string InvalidCakePrice = "Price of cake must be only valid numbers!";
        private const string InternalDbError = "Invalid request sent to the database";
        private const string PictureOfProductNotValid = "{0} /r/n This is not a valid Url!";
        private const string NotFoundProduct = "Product with name {0} not found!";
        
        public IHttpResponse GetRegister(IHttpRequest request)
        {
            return this.View("Register");
        }

        public IHttpResponse PostRegister(IHttpRequest request)
        {
            var username = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();
            var confirmPassword = request.FormData["confirmPassword"].ToString();
            var fullName = request.FormData["fullName"].ToString();

            if (string.IsNullOrWhiteSpace(username) ||
                !Regex.IsMatch(username, @"^([A-Za-z0-9_.-]+)$") ||
                username.Length < 4)
            {
                return this.BadRequest();
            }

            var user = this.Db.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                return new HtmlResult(UserAlreadyExists, HttpResponseStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return new HtmlResult(EmptyPasswordOrLengthNotEnough, HttpResponseStatusCode.BadRequest);
            }

            if (password != confirmPassword)
            {
                return new HtmlResult(PasswordsDontMatchMessage, HttpResponseStatusCode.BadRequest);
            }

            var hashedPassword = this.HashService.Hash(password);

            var currentUser = new User
            {
                Name = fullName,
                Username = username,
                Password = hashedPassword
            };

            this.Db.Users.Add(currentUser);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return new HtmlResult(InternalDbError, HttpResponseStatusCode.InternalServerError);
            }

            var response = new RedirectResult("/");

            return response;
        }

        public IHttpResponse GetLogin(IHttpRequest request)
        {
            return this.View("Login");
        }

        public IHttpResponse PostLogin(IHttpRequest request)
        {
            var username = request.FormData["username"].ToString();
            var password = request.FormData["password"].ToString();

            var hashedPassword = this.HashService.Hash(password);


            if (!this.Db.Users.Any(u => u.Username == username && u.Password == hashedPassword))
            {
                return new RedirectResult("/register");
            }

            var hashedUser = this.CookieService.GetUserCookie(username);

            var cookie = new HttpCookie(".auth-cookie", hashedUser);

            var response = new RedirectResult("/");
            response.AddCookie(cookie);

            return response;
        }

        public IHttpResponse LogOut(IHttpRequest request)
        {
            var cookie = GetAuthCookie(request);

            if (cookie == null)
            {
                //return new HtmlResult(UserNotLoggedIn, HttpResponseStatusCode.BadRequest);
                return new RedirectResult("/register");
            }

            cookie.Delete();

            var response = new RedirectResult("/");

            response.AddCookie(cookie);

            return response;
        }

        public IHttpResponse GetProfileInfo(IHttpRequest request)
        {
            var cookie = GetAuthCookie(request);

            if (cookie == null)
            {
                return new RedirectResult("/register");
            }

            var username = this.CookieService.GetUserData(cookie.Value);

            var user = this.Db.Users.FirstOrDefault(u => u.Username == username);

            var sb = new StringBuilder();

            sb.AppendLine(@"<a href=""/"">Back to Home</a>");
            sb.AppendLine(@"<h1>My Profile</h1>");
            sb.AppendLine($@"<p>Name: {user.Name.Replace('+', ' ')}</p>");
            sb.AppendLine($@"<p>Registered On: {user.DateOfRegistration:dd-MM-yyyy}</p>");
            sb.AppendLine($@"<p>Orders Count: {user.Orders.Count}</p>");

            var result = sb.ToString().Trim();

            var response = new HtmlResult(result, HttpResponseStatusCode.Ok);

            return response;
        }

        public IHttpResponse GetCake(IHttpRequest request)
        {
            var cookie = GetAuthCookie(request);
            if (cookie == null)
            {
                return new RedirectResult("/register");
            }

            return this.View("AddCake");
        }

        public IHttpResponse PostCake(IHttpRequest request)
        {
            var nameOfProduct = request.FormData["productName"].ToString();
            var priceOfProduct = request.FormData["productPrice"].ToString();
            var productUrl = request.FormData["pictureUrl"].ToString();

            var isItValidPrice = decimal.TryParse(priceOfProduct, out var parsedPriceOfCake);
            if (!isItValidPrice)
            {
                return new HtmlResult(InvalidCakePrice, HttpResponseStatusCode.BadRequest);
            }

            if (!ValidateUrl(productUrl))
            {
                return new HtmlResult(string.Format(PictureOfProductNotValid, productUrl), HttpResponseStatusCode.BadRequest);
            }

            var product = new Product()
            {
                Name = nameOfProduct,
                Price = parsedPriceOfCake,
                ImageUrl = productUrl
            };

            this.Db.Products.Add(product);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return new HtmlResult(InternalDbError, HttpResponseStatusCode.InternalServerError);
            }

            var response = new RedirectResult("/");
            return response;
        }

        public IHttpResponse GetBrowseCakes(IHttpRequest request)
        {
            var authCookie = GetAuthCookie(request);
            if (authCookie == null) return new RedirectResult("/register");

            return this.View("Search");
        }

        public IHttpResponse PostBrowseCakes(IHttpRequest request)
        {
            var productNameToFind = request.FormData["searchField"].ToString();

            var product = this.Db.Products.FirstOrDefault(c => c.Name == productNameToFind);
            if (product == null)
            {
                return new HtmlResult(string.Format(NotFoundProduct, productNameToFind), HttpResponseStatusCode.NotFound);
            }

            var nameOfCake = product.Name.Replace('+', ' ');

            var sb = new StringBuilder();
            sb.AppendLine(
                $@"<p><a href=""/{nameOfCake}"">{nameOfCake}</a> ${product.Price} <input type=""submit""  value=""Order"" /></p>");

            var result = sb.ToString().Trim();

            var response = new HtmlResult(Encoding.UTF8.GetString(this.View("Search").Content) + result, HttpResponseStatusCode.Ok);

            return response;
        }

        private bool ValidateUrl(string cakeUrl)
        {
            var decodedUrl = WebUtility.UrlDecode(cakeUrl);
            bool result = Uri.TryCreate(decodedUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        private HttpCookie GetAuthCookie(IHttpRequest request)
        {
            var cookie = request.Cookies.FirstOrDefault(c => c.Key == ".auth-cookie");

            return cookie;
        }
    }
}
