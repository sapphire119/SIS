namespace SIS.WebServer.Api
{
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses.Interfaces;

    public interface IHttpHandler
    {
        IHttpResponse Handle(IHttpRequest request);
    }
}
