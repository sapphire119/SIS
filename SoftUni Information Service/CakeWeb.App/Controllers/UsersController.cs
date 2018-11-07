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
    using System.Collections.Generic;

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
            

            if (string.IsNullOrWhiteSpace(username) || !Regex.IsMatch(username, UsernameRegex) || username.Length < UsernameReqLength)
            {
                return this.ErrorView(InvalidUsernameFormat);
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

            var currentUser = new User(fullName, username, hashedPassword);

            this.Db.Users.Add(currentUser);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return new HtmlResult(InternalDbError, HttpResponseStatusCode.InternalServerError);
            }

            var response = new RedirectResult("/Users/Login");

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
                return this.ErrorView("Invalid Username or Password.");
            }

            var hashedUser = this.CookieService.GetUserCookie(username);

            var cookie = new HttpCookie(".auth-cookie", hashedUser);
            cookie.SetPath("/");

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
                return new RedirectResult("/Users/Register");
            }

            request.Session.ClearParameters();

            cookie.Delete();
            cookie.SetPath("/");

            var response = new RedirectResult("/");

            response.AddCookie(cookie);

            return response;
        }

        public IHttpResponse GetProfileInfo(IHttpRequest request)
        {
            var cookie = GetAuthCookie(request);

            if (cookie == null)
            {
                return new RedirectResult("/Users/Register");
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

        private HttpCookie GetAuthCookie(IHttpRequest request)
        {
            var cookie = request.Cookies.FirstOrDefault(c => c.Key == ".auth-cookie");

            return cookie;
        }
    }
}
