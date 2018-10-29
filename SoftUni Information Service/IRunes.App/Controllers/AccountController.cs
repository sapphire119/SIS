using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Intefaces;
using SIS.HTTP.Responses.Interfaces;
using SIS.WebServer.Results;
using System.Linq;
using System.Text.RegularExpressions;

namespace IRunes.App.Controllers
{
    public class AccountController : BaseController
    {
        private const string EmailPatternRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        private const string UsernameRegex = @"^[a-zA-Z.-]+[0-9]*$";

        private const int UsernameOrEmailLength = 4;

        private const string InvalidCredentials = "Invalid credentials!";

        private const string InvalidUsernameEmailOrPassword = "Invalid username/email or password";

        public IHttpResponse GetLoginView(IHttpRequest request)
        {
            return this.View("UsersLoggin");
        }

        public IHttpResponse PostLoginView(IHttpRequest request)
        {
            var usernameOrEmail = request.FormData["usernameOrEmail"].ToString().Trim();

            var password = request.FormData["password"].ToString();

            var isEmail = Regex.IsMatch(usernameOrEmail, EmailPatternRegex);

            var isUsername = Regex.IsMatch(usernameOrEmail, UsernameRegex);

            if ((!isEmail && !isUsername) || usernameOrEmail.Length < UsernameOrEmailLength)
            {
                return this.ErrorView(InvalidCredentials, HttpResponseStatusCode.BadRequest);
            }

            var hashedPassword = this.HashService.Hash(password);

            var user = this.Db.Users.FirstOrDefault(
                u =>
                    (u.Username == usernameOrEmail && u.Password == password) ||
                    (u.Email == usernameOrEmail && u.Password == password));

            if (user == null)
            {
                return this.ErrorView(InvalidUsernameEmailOrPassword, HttpResponseStatusCode.BadRequest);
            }

            var cookieContent = this.UserCookieService.GetUserCookie(user.Username);

            var response = new RedirectResult("/Home/Index");

            response.AddCookie(new HttpCookie(".auth-cookie", cookieContent));

            return response;
        }

    }
}
