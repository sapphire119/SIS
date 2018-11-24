namespace SIS.Framework.Attributes
{
    using SIS.Framework.Attributes.Methods;
    using SIS.HTTP.Enums;

    using System;

    public class HttpPostAttribute : HttpMethodAttribute
    {
        public override bool IsValid(string requestMethod)
        {
            return Enum.TryParse(requestMethod.ToUpper(), out HttpRequestMethod method);
        }
    }
}
