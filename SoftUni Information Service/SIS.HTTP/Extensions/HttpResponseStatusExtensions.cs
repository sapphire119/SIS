namespace SIS.HTTP.Extensions
{
    using HTTP.Enums;
    using System;
    using Interfaces;
    using System.Net;

    //Втори вариант
    //public static class HttpResponseStatusExtensions
    //{
    //    public static string GetResponseLine(this HttpStatusCode statusCode)
    //        => $"{(int)statusCode} {statusCode}";
    //}

    public class HttpResponseStatusExtensions : IHttpResponseStatusExtensions
    {
        private HttpResponseStatusCode statusCode;

        public HttpResponseStatusExtensions(HttpResponseStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }

        public string GetResponseLine()
        {
            switch (this.statusCode)
            {
                case HttpResponseStatusCode.Ok:
                    return $"{(int)this.statusCode} {this.statusCode.ToString().ToUpper()}";
                case HttpResponseStatusCode.Created:
                    return $"{(int)this.statusCode} {this.statusCode.ToString()}";
                case HttpResponseStatusCode.Found:
                    return $"{(int)this.statusCode} {this.statusCode.ToString()}";
                case HttpResponseStatusCode.SeeOther:
                    return $"{(int)this.statusCode} {this.statusCode.ToString().Insert(3, " ")}";
                case HttpResponseStatusCode.BadRequest:
                    return $"{(int)this.statusCode} {this.statusCode.ToString().Insert(3, " ")}";
                case HttpResponseStatusCode.Unauthorized:
                    return $"{(int)this.statusCode} {this.statusCode.ToString()}";
                case HttpResponseStatusCode.Forbidden:
                    return $"{(int)this.statusCode} {this.statusCode.ToString()}";
                case HttpResponseStatusCode.NotFound:
                    return $"{(int)this.statusCode} {this.statusCode.ToString().Insert(3, " ")}";
                case HttpResponseStatusCode.InternalServerError:
                    return $"{(int)this.statusCode} Internal Server Error";
                default:
                    throw new ArgumentException("Status Code not found!");
            }
        }
    }
}
