namespace SIS.MvcFramework.ViewEngine
{
    using SIS.MvcFramework.ViewEngine.Contracts;

    public class ViewEngine : IViewEngine
    {
        public string GetHtml(string viewCode)
        {
            return viewCode;
        }
    }

}
