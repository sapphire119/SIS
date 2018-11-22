namespace SIS.HTTP.Responses
{
    using System;
    using System.Text;
    using System.Linq;

    using SIS.HTTP.Common;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Extensions;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Headers.Intefaces;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.HTTP.Cookies.Interfaces;
    using SIS.HTTP.Cookies;

    public class HttpResponse : IHttpResponse
    {
        public HttpResponse() { }

        public HttpResponse(HttpResponseStatusCode statusCode)
        {
            this.Headers = new HttpHeaderCollection();

            this.Cookies = new HttpCookieCollection();

            this.Content = new byte[0];

            this.StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; private set; }

        public IHttpCookieCollection Cookies { get; }

        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header)
        {
            this.Headers.Add(header);
        }

        public void AddCookie(HttpCookie cookie)
        {
            this.Cookies.Add(cookie);
        }

        public byte[] GetBytes()
        {
            //var test = Encoding.UTF8.GetBytes(this.ToString()).Concat(this.Content).ToArray();

            var byteArr = this.ToString().ToCharArray().Select(e => (byte)e).ToArray();

            var resultingArr = new byte[byteArr.Length + this.Content.Length];

            Array.Copy(byteArr, resultingArr, byteArr.Length);
            Array.Copy(this.Content, 0, resultingArr, byteArr.Length, this.Content.Length);

            return resultingArr;
        }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            
            sb
            .AppendLine($"{GlobalConstants.HttpOneProtocolFragment} {this.StatusCode.GetResponseLine()}")
            .AppendLine($"{this.Headers}");
            
            if (this.Cookies.HasCookies())
            {
                foreach (var httpCookie in this.Cookies)
                {
                    sb.AppendLine($"{GlobalConstants.SetCookieHeaderKey}: {httpCookie}");
                }
            }

            sb.AppendLine();

            var result = sb.ToString();

            return result;
        }
    }
}
