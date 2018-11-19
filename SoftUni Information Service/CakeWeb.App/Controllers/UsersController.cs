﻿namespace CakeWeb.App.Controllers
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

        public IHttpResponse GetRegister()
        {
            return this.View("Register");
        }

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

        public IHttpResponse GetLogin()
        {
            return this.View("Login");
        }

        public IHttpResponse PostLogin()
        {
            var username = this.Request.FormData["username"].ToString();
            var password = this.Request.FormData["password"].ToString();

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

        public IHttpResponse LogOut()
        {
            var cookie = GetAuthCookie();

            if (cookie == null)
            {
                //return new HtmlResult(UserNotLoggedIn, HttpResponseStatusCode.Badthis.Request);
                return new RedirectResult("/Users/Register");
            }

            this.Request.Session.ClearParameters();

            cookie.Delete();
            cookie.SetPath("/");

            var response = new RedirectResult("/");

            response.AddCookie(cookie);

            return response;
        }

        public IHttpResponse GetProfileInfo()
        {
            var cookie = GetAuthCookie();

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

        private HttpCookie GetAuthCookie()
        {
            var cookie = this.Request.Cookies.FirstOrDefault(c => c.Key == ".auth-cookie");

            return cookie;
        }
    }
}
