namespace SIS.HTTP.Cookies
{
    using System;

    public class HttpCookie
    {
        private const int HttpCookieDefaultExpirationDays = 3;

        public HttpCookie(string key, string value, int expires = HttpCookieDefaultExpirationDays)
        {
            this.Key = key;
            this.Value = value;
            this.IsNew = true;
            this.Expires = DateTime.UtcNow.AddDays(expires);
        }

        public HttpCookie(string key, string value, bool isNew, int expires = HttpCookieDefaultExpirationDays)
            : this(key, value, expires)
        {
            this.IsNew = isNew;
        }

        public string Key { get; }

        public string Value { get; }

        public DateTime Expires { get; private set; }

        public bool IsNew { get; }

        public void Delete() => this.Expires = DateTime.UtcNow.AddDays(-1);

        public string Path { get; private set; }

        public void SetPath(string path = null)
        {
            this.Path = path == null ? null : $"; Path={path}";
        }

        public override string ToString()
        {
            return $"{this.Key}={this.Value}; Expires={this.Expires:R}{this.Path}";
        }
    }
}
