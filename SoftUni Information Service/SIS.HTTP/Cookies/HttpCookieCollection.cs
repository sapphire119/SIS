namespace SIS.HTTP.Cookies
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private readonly ICollection<HttpCookie> cookies;

        public HttpCookieCollection()
        {
            this.cookies = new List<HttpCookie>();
        }

        public void Add(HttpCookie cookie)
        {
            this.cookies.Add(cookie);
        }

        public bool ContainsCookie(string key)
        {
            return this.cookies.Any(c => c.Key == key);
        }

        public HttpCookie GetCookie(string key)
        {
            var cookie = this.cookies.FirstOrDefault(c => c.Key == key);

            return cookie;
        }

        
        public bool HasCookies()
        {
            return this.cookies.Any();
        }

        public override string ToString()
        {
            return string.Join("; ", this.cookies);
        }


        public IEnumerator<HttpCookie> GetEnumerator()
        {
            foreach (var cookie in this.cookies)
            {
                yield return cookie;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
