using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SIS.HTTP.Extensions
{
    public static class DecoderExtension
    {
        public static string UrlDecode(this string url)
        {
            return WebUtility.UrlDecode(url);
        }
    }
}
