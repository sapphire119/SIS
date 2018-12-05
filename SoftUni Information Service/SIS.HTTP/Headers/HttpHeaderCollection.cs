namespace SIS.HTTP.Headers
{
    using System.Linq;
    using System.Collections.Generic;

    using Intefaces;
    using System.Text;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly Dictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, HttpHeader>();
        }

        public void Add(HttpHeader header)
        {
            this.headers[header.Key] = header;
        }

        public bool ContainsHeader(string key)
        {
            return this.headers.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            var header = this.headers.SingleOrDefault(h => h.Key == key).Value;

            return header;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var header in this.headers)
            {
                sb.AppendLine(header.Value.ToString());
            }

            return sb.ToString().Trim();
        }
    }
}
