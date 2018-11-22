namespace SIS.HTTP.Extensions
{
    using HTTP.Enums;
    using System;

    //Втори вариант
    //public static class HttpResponseStatusExtensions
    //{
    //    public static string GetResponseLine(this HttpStatusCode statusCode)
    //        => $"{(int)statusCode} {statusCode}";
    //}

    public static class HttpResponseStatusExtensions
    {

        public static string GetResponseLine(this HttpResponseStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpResponseStatusCode.Ok:
                    return $"{(int)statusCode} {statusCode.ToString().ToUpper()}";
                case HttpResponseStatusCode.Created:
                    return $"{(int)statusCode} {statusCode.ToString()}";
                case HttpResponseStatusCode.Found:
                    return $"{(int)statusCode} {statusCode.ToString()}";
                case HttpResponseStatusCode.SeeOther:
                    return $"{(int)statusCode} {statusCode.ToString().Insert(3, " ")}";
                case HttpResponseStatusCode.BadRequest:
                    return $"{(int)statusCode} {statusCode.ToString().Insert(3, " ")}";
                case HttpResponseStatusCode.Unauthorized:
                    return $"{(int)statusCode} {statusCode.ToString()}";
                case HttpResponseStatusCode.Forbidden:
                    return $"{(int)statusCode} {statusCode.ToString()}";
                case HttpResponseStatusCode.NotFound:
                    return $"{(int)statusCode} {statusCode.ToString().Insert(3, " ")}";
                case HttpResponseStatusCode.InternalServerError:
                    return $"{(int)statusCode} Internal Server Error";
                default:
                    throw new ArgumentException("Status Code not found!");
            }
        }
    }
}
