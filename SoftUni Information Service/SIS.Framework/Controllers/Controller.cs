namespace SIS.Framework.Controllers
{
    using SIS.Framework.ActionResults;
    using SIS.Framework.ActionResults.Interfaces;
    using SIS.Framework.Utilities;
    using SIS.Framework.Views;
    using SIS.HTTP.Requests.Intefaces;

    using System.Runtime.CompilerServices;

    public abstract class Controller
    {
        public IHttpRequest Request { get; set; }

        protected IViewable View([CallerMemberName] string viewName = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            var fullyQualifiedName = ControllerUtilities.GetViewFullQualifiedName(controllerName, viewName);

            var view = new View(fullyQualifiedName);

            return new ViewResult(view);
        }

        protected IRedirectable RedirectToAction(string redirectUrl)
            => new RedirectResult(redirectUrl);
    }
}
