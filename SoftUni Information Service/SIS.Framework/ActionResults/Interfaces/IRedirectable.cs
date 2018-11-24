namespace SIS.Framework.ActionResults.Interfaces
{
    public interface IRedirectable : IActionResult
    {
        string RedirectUrl { get; }
    }
}
