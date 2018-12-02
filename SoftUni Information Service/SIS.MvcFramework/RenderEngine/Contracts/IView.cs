namespace SIS.MvcFramework.RenderEngine.Contracts
{
    public interface IView<T>
    {
        string GetHtml(T model);
    }
}
