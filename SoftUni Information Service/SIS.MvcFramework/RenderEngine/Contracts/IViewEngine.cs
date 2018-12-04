namespace SIS.MvcFramework.RenderEngine.Contracts
{
    public interface IViewEngine
    {
        string GetHtml<T>(string viewName, string viewCode, T model, string user);
    }
}
