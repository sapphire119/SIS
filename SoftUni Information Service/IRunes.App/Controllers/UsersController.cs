namespace IRunes.App.Controllers
{
    using IRunes.App.Extensions;
    using IRunes.Models;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.HTTP.Sessions;
    using SIS.WebServer.Results;
    using System;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;


    public class UsersController : BaseController
    {
        private const string EmailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        private const string UsernameRegex = @"^[a-zA-Z.-]+[0-9]*$";

        private const int UsernameOrEmailLength = 4;

        private const string InvalidCredentials = "Invalid credentials!";

        private const string InvalidUsernameEmailOrPassword = "Invalid username/email or password";

        private const string InvalidUsernameLength = "Please provide valid username with length of 4 or more characters";
        private const string UserAlreadyExists = "User with the same username already exists";

        private const string InvalidPasswordLength = "Password must be at least 6 symbols";

        private const string PasswordsDontMatchMessage = "Passwords don't match";

        private const string InvalidEmail = "Invalid email!";

        public IHttpResponse GetLoginView(IHttpRequest request)
        {
            if (request.Cookies.ContainsCookie(".auth-cookie"))
            {
                return this.ErrorView("User already logged in!", HttpResponseStatusCode.BadRequest);
            }

            return this.View("Login").ApplyLayout(request);
        }

        public IHttpResponse GetRegisterView(IHttpRequest request)
        {
            return this.View("Register").ApplyLayout(request);
        }

        public IHttpResponse PostLoginView(IHttpRequest request)
        {
            var usernameOrEmail = request.FormData["usernameOrEmail"].ToString().Trim();

            var password = request.FormData["password"].ToString();

            var isEmail = Regex.IsMatch(WebUtility.UrlDecode(usernameOrEmail), EmailRegex);
            
            var isUsername = Regex.IsMatch(usernameOrEmail, UsernameRegex);

            if ((!isEmail && !isUsername) || usernameOrEmail.Length < UsernameOrEmailLength)
            {
                return this.ErrorView(InvalidCredentials);
            }

            var hashedPassword = this.HashService.Hash(password);

            var user = this.Db.Users.FirstOrDefault(
                u =>
                    (u.Username == usernameOrEmail && u.Password == hashedPassword) ||
                    (u.Email == usernameOrEmail && u.Password == hashedPassword));

            if (user == null)
            {
                return this.ErrorView(InvalidUsernameEmailOrPassword);
            }

            var cookieContent = this.UserCookieService.GetUserCookie(usernameOrEmail);

            //request.Session.AddParamter("username", cookieContent);

            var cookie = new HttpCookie(".auth-cookie", cookieContent);
            cookie.SetPath("/");

            var response = new RedirectResult("/");
            response.AddCookie(cookie);

            return response;
        }

        public IHttpResponse PostRegisterView(IHttpRequest request)
        {
            var username = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();
            var confirmPassword = request.FormData["confirmPassword"].ToString();
            var email = request.FormData["email"].ToString();

            //Validate
            if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
            {
                return  this.ErrorView(InvalidUsernameLength);
            }

            if (this.Db.Users.Any(u => u.Username == username))
            {
                return this.ErrorView(UserAlreadyExists);
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return this.ErrorView(InvalidPasswordLength);
            }

            if (password != confirmPassword)
            {
                return this.ErrorView(PasswordsDontMatchMessage);
            }
            
            var isEmailValid = Regex.IsMatch(WebUtility.UrlDecode(email), EmailRegex);

            if (!isEmailValid) return this.ErrorView(InvalidEmail);
            //Hash Password
            var hashedPassword = this.HashService.Hash(password);

            //Create User
            var user = new User(username, hashedPassword, email);

            this.Db.Users.Add(user);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception e)
            {
                // TODO: Log Error
                return this.ErrorView(e.Message, HttpResponseStatusCode.InternalServerError);
            }

            // TODO: Login

            //Redirect
            return new RedirectResult("/");
            //1. Validate!
            //2. Generate password hash;
            //3. Create User
            //4. Redirect to Home Page
        }

        public IHttpResponse GetLogout(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth-cookie")) return new RedirectResult("/Users/Login");

            var cookie = request.Cookies.GetCookie(".auth-cookie");

            cookie.Delete();
            cookie.SetPath("/");
            var response = new RedirectResult("/");

            response.AddCookie(cookie);

            return response;
        }
    }
}
