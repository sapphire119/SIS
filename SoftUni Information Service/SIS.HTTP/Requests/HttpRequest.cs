namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intefaces;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Cookies.Interfaces;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Extensions;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Headers.Intefaces;
    using SIS.HTTP.Sessions.Interfaces;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public IHttpSession Session { get; set; }

        public HttpRequestMethod RequestMethod { get; private set; }


        private bool IsValidRequestLine(string[] requestLine)
        {
            return requestLine.Length == 3 && requestLine[2] == GlobalConstants.HttpOneProtocolFragment;
        }

        private bool IsValidRequestQueryString(string queryString, string[] queryParameters)
        {
            var condition = string.IsNullOrWhiteSpace(queryString) || queryParameters.Length <= 1;

            return condition;
        }

        private void ParseRequestMethod(string[] requestLine)
        {
            var isItAValidEnum = Enum.TryParse(requestLine[0].Capitalize(), out HttpRequestMethod method);
            if (!isItAValidEnum)
            {
                throw new BadRequestException();
            }

            this.RequestMethod = method;
        }

        private void ParseRequestUrl(string[] requestLine)
        {
            this.Url = requestLine[1];
        }

        private void ParseRequestPath()
        {
            var splitUrl = this.Url.Split("?").ToArray();
            this.Path = splitUrl[0];
        }

        private void ParseHeaders(string[] requestContent)
        {
            if (!requestContent.Any()) throw new BadRequestException();

            var formattedRequestContent = requestContent.TakeWhile(e => !string.IsNullOrWhiteSpace(e)).ToArray();

            var tempDict = new Dictionary<string, string>();

            foreach (var item in formattedRequestContent)
            {
                var firstPartOfHeader = string.Concat(item.TakeWhile(e => e != ':'));
                var secondPartOfHeader = item.Substring(firstPartOfHeader.Length + 2);

                tempDict.Add(firstPartOfHeader, secondPartOfHeader);
            }

            if (!tempDict.ContainsKey(GlobalConstants.HostHeaderKey)) throw new BadRequestException();

            foreach (var header in tempDict)
            {
                var httpHeader = new HttpHeader(header.Key, header.Value);
                this.Headers.Add(httpHeader);
            }
        }

        private void ParseQueryParameters()
        {
            if (this.Path.Length == this.Url.Length) return;

            var queryString = string.Concat(this.Url.Split("?").Skip(1).ToArray());
            var queryCollection = queryString.Split("&").ToArray();

            if (string.IsNullOrWhiteSpace(queryString)) return;

            foreach (var query in queryCollection)
            {
                var tokens = query.Split("=").ToArray();
                if (this.IsValidRequestQueryString(query, tokens)) throw new BadRequestException();
                this.QueryData.Add(tokens[0], tokens[1]);
            }
        }

        private void ParseFormDataParameters(string[] splitRequestContent)
        {
            var firstCondition = splitRequestContent[splitRequestContent.Length - 1] == string.Empty;

            var secondCondition = splitRequestContent[splitRequestContent.Length - 2] == string.Empty;

            if (firstCondition && secondCondition) return;

            var formData = splitRequestContent[splitRequestContent.Length - 1];

            var formDataCollection = formData.Split("&");
            foreach (var item in formDataCollection)
            {
                var kvp = item.Split("=");
                var key = kvp[0];
                var value = kvp[1];

                this.FormData.Add(key, value);
            }
        }

        private void ParseRequestParameters(string[] splitRequestContent)
        {
            this.ParseQueryParameters();
            this.ParseFormDataParameters(splitRequestContent);
        }

        private void ParseCookies()
        {
            if (!this.Headers.ContainsHeader(GlobalConstants.CookieHeaderKey)) return;

            var cookieHeader = this.Headers.GetHeader(GlobalConstants.CookieHeaderKey);

            var cookies = cookieHeader.Value.Split("; ");

            foreach (var cookieString in cookies)
            {
                var cookieTokens = cookieString.Split("=", 2, StringSplitOptions.RemoveEmptyEntries);

                if (cookieTokens.Length != 2) return;

                var keyToken = cookieTokens[0];

                var valueToken = cookieTokens[1];

                var cookie = new HttpCookie(keyToken, valueToken);

                this.Cookies.Add(cookie);
            }

        }

        private void ParseRequest(string requestString)
        {
            string[] splitRequestContent = requestString
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            string[] requestLine = splitRequestContent[0].Trim().
                Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();

            this.ParseHeaders(splitRequestContent.Skip(1).ToArray());
            this.ParseCookies();

            this.ParseRequestParameters(splitRequestContent.Skip(1).ToArray());
        }
    }
}
