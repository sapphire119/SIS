﻿namespace SIS.HTTP.Cookies.Interfaces
{
    using System.Collections.Generic;

    public interface IHttpCookieCollection : IEnumerable<HttpCookie>
    {
        void Add(HttpCookie cookie);

        bool ContainsCookie(string key);

        HttpCookie GetCookie(string key);

        bool HasCookies();
    }
}
