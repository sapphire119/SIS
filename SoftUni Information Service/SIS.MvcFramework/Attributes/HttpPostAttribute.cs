namespace SIS.MvcFramework.Attributes
{
    using System;
    using SIS.HTTP.Enums;

    public class HttpPostAttribute : HttpAttribute
    {
        public HttpPostAttribute(string path)
            : base(path) { }

        public override HttpRequestMethod RequestMethod => HttpRequestMethod.Post;
    }
}
