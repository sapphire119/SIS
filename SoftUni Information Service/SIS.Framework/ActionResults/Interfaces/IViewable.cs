namespace SIS.Framework.ActionResults.Interfaces
{
    public interface IViewable : IActionResult
    {
        IRenderable View { get; set; }
    }
}
