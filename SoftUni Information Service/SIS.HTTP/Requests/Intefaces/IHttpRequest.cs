namespace SIS.HTTP.Requests.Intefaces
{
    using SIS.HTTP.Cookies.Interfaces;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers.Intefaces;
    using SIS.HTTP.Sessions.Interfaces;
    using System.Collections.Generic;

    public interface IHttpRequest
    {
        string Path { get; }

        string Url { get; }

        Dictionary<string, object> FormData { get; }

        Dictionary<string, object> QueryData { get; }

        IHttpHeaderCollection Headers { get; }

        IHttpCookieCollection Cookies { get; }

        IHttpSession Session { get; set; }

        HttpRequestMethod RequestMethod { get; }
    }
}
