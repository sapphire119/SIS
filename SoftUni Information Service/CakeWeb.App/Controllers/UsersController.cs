namespace CakeWeb.App.Controllers
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses.Interfaces;

    using System.Text.RegularExpressions;
    using System.Linq;
    using System;
    using CakesWeb.Models;
    using SIS.HTTP.Cookies;
    using System.Collections.Generic;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.Services.Contracts;

    public class UsersController : BaseController
    {
        private const string PasswordsDontMatchMessage = "Passwords do not match";
        private const string EmptyPasswordOrLengthNotEnough = "Password is empty or length is below 6 characters";
        private const string UserAlreadyExists = "User already exists.";
        private const string InvalidUsernameOrPassword = "No user exists with given username or password";
        private const string UserNotLoggedIn = "User not logged in!";
        private const string InvalidUsernameFormat = "Username not valid!";

        private const string UsernameRegex = @"^([A-Za-z0-9_.-]+)$";

        private const int UsernameReqLength = 4;

        private readonly IHashService hashService;

        public UsersController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        [HttpGet("/Users/Register")]
        public IHttpResponse GetRegister()
        {
            return this.View("Register");
        }

        [HttpPost("/Users/Register")]
        public IHttpResponse PostRegister()
        {
            var username = this.Request.FormData["username"].ToString().Trim();
            var password = this.Request.FormData["password"].ToString();
            var confirmPassword = this.Request.FormData["confirmPassword"].ToString();
            var fullName = this.Request.FormData["fullName"].ToString();
            

            if (string.IsNullOrWhiteSpace(username) || !Regex.IsMatch(username, UsernameRegex) || username.Length < UsernameReqLength)
            {
                return this.ErrorView(InvalidUsernameFormat);
            }

            var user = this.Db.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                return this.Html(UserAlreadyExists);
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return this.Html(EmptyPasswordOrLengthNotEnough);
            }

            if (password != confirmPassword)
            {
                return this.Html(PasswordsDontMatchMessage);
            }

            var hashedPassword = this.hashService.Hash(password);

            var currentUser = new User(fullName, username, hashedPassword);

            this.Db.Users.Add(currentUser);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return this.ErrorView(InternalDbError, HttpResponseStatusCode.InternalServerError);
            }

            return this.Redirect("/Users/Login");
        }

        [HttpGet("/Users/Login")]
        public IHttpResponse GetLogin()
        {
            return this.View("Login");
        }

        [HttpPost("/Users/Login")]
        public IHttpResponse PostLogin()
        {
            var username = this.Request.FormData["username"].ToString();
            var password = this.Request.FormData["password"].ToString();

            var hashedPassword = this.hashService.Hash(password);

            if (!this.Db.Users.Any(u => u.Username == username && u.Password == hashedPassword))
            {
                return this.ErrorView("Invalid Username or Password.");
            }

            var hashedUser = this.CookieService.GetUserCookie(username);

            var cookie = new HttpCookie(".auth-cookie", hashedUser);

            cookie.SetCookiePath("/");
            
            this.Response.AddCookie(cookie);

            return this.Redirect("/");
        }

        [HttpGet("/Users/Logout")]
        public IHttpResponse LogOut()
        {
            var cookie = GetAuthCookie();

            if (cookie == null)
            {
                //return this.Html(UserNotLoggedIn, HttpResponseStatusCode.Badthis.Request);
                return this.Redirect("/Users/Register");
            }

            this.Request.Session.ClearParameters();

            cookie.Delete();
            cookie.SetCookiePath("/");

            this.Response.AddCookie(cookie);

            return this.Redirect("/");
        }

        [HttpGet("/Users/Details")]
        public IHttpResponse GetProfileInfo()
        {
            var cookie = GetAuthCookie();

            if (cookie == null)
            {
                return this.Redirect("/Users/Register");
            }

            var username = this.CookieService.GetUserData(cookie.Value);

            var user = this.Db.Users.FirstOrDefault(u => u.Username == username);

            var viewBag = new Dictionary<string, string>();

            viewBag["fullName"] = user.Name.Replace("+", " ");
            viewBag["dateOfReg"] = user.DateOfRegistration.ToString(@"dd-MM-yyyy");
            viewBag["orderCount"] = user.Orders.Count.ToString();

            var response = this.View("Details", viewBag);

            return response;
        }

        private HttpCookie GetAuthCookie()
        {
            var cookie = this.Request.Cookies.FirstOrDefault(c => c.Key == ".auth-cookie");

            return cookie;
        }
    }
}
