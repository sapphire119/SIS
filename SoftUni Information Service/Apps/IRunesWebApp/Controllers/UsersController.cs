namespace IRunesWebApp.Controllers
{
    using IRunesWebApp.Services.Contracts;
    using IRunesWebApp.ViewModels;
    using IRunesWebApp.ViewModels.Users;
    using SIS.Framework.ActionsResults.Base;
    using SIS.Framework.Attributes.Action;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Controllers;
    using SIS.Framework.Security;
    using SIS.HTTP.Enums;
    using SIS.WebServer.Results;
    using System;

    public class UsersController : Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService, )
        {
            this.usersService = usersService;
        }

        public IActionResult Login()
        {
            this.SignIn(new IdentityUser
            {
                Username = "Pesho",
                Password = "123456",
                //Roles = new string[] { "Admin" }
            });
            return this.View();
        }

        [Authorize/*("Admin")*/]
        public IActionResult Authorized()
        {
            this.ViewModel["username"] = this.Identity.Username;

            return this.View();
        }
     
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid.HasValue || !ModelState.IsValid.Value)
            {
                return this.RedirectToAction("/users/login");
            }

            var userExists = this.usersService
                .ExistsByUsernameAndPassword(
                    model.Username,
                    model.Password);

            if (!userExists)
            {
                return this.RedirectToAction("/users/login");
            }

            //var response = new RedirectResult("/home/index");
            //this.SignInUser(username, response, request);
            this.Request.Session.AddParameter("username", model.Username);
            return this.RedirectToAction("/home/index");
        }

        public IActionResult Register() => this.View();

        [HttpPost]
        public IActionResult Register(RegisterInputModel model)
        {

            // Validate
            //if (string.IsNullOrWhiteSpace(userName) || userName.Length < 4)
            //{
            //    return new BadRequestResult("Please provide valid username with length of 4 or more characters.");
            //}

            //if (this.Context.Users.Any(x => x.Username == userName))
            //{
            //    return new BadRequestResult("User with the same name already exists.");
            //}

            //if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            //{
            //    return new BadRequestResult("Please provide password of length 6 or more.");
            //}

            if (model.Password != model.ConfirmPassword)
            {
                throw new Exception("Passwords do not match.");
            }

            // Hash password
            //var hashedPassword = this.usersService.Hash(password);

            // Create user
            var user = new RegisterUserViewModel
            {
                Username = model.Username,
                HashedPassword = model.Password + "myAppSalt123",
            };

            this.usersService.AddUserToContext(user);
            this.Context.Users.Add(user);

            try
            {
                this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                // TODO: Log error
                return new BadRequestResult(
                    e.Message,
                    HttpResponseStatusCode.InternalServerError);
            }

            var response = new RedirectResult("/");
            this.SignInUser(userName, response, request);

            // Redirect
            return response;
        }


    }
}
