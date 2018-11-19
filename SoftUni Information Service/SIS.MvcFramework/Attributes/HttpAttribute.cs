namespace SIS.MvcFramework.Attributes
{
    using SIS.HTTP.Enums;
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class HttpAttribute : Attribute
    {
        public HttpAttribute(string path)
        {
            this.Path = path;
        }

        public string Path { get; }

        public abstract HttpRequestMethod RequestMethod { get; }
    }
}
