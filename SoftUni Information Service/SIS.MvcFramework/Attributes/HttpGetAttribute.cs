namespace SIS.MvcFramework.Attributes
{
    using System;
    using SIS.HTTP.Enums;

    public class HttpGetAttribute : HttpAttribute
    {
        public HttpGetAttribute(string path)
            : base(path) { }

        public override HttpRequestMethod RequestMethod => HttpRequestMethod.Get;
    }
}
