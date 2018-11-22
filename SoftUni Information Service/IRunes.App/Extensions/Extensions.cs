using SIS.HTTP.Requests.Intefaces;
using SIS.HTTP.Responses.Interfaces;
using System.IO;
using System.Text;

namespace IRunes.App.Extensions
{
    public static class Extensions
    {
        private const string FileRelativePath = "../../../";

        public static IHttpResponse ApplyLayout(this IHttpResponse response, IHttpRequest request)
        {
            var layout = File.ReadAllText(string.Concat(FileRelativePath, "Views/", "_Layout.html"));

            layout = layout.Replace("@Render.Body()", Encoding.UTF8.GetString(response.Content));


            string loginView;
            if (request.Cookies.ContainsCookie(".auth-cookie"))
            {
                loginView = File.ReadAllText(string.Concat(FileRelativePath, "Views/", "Menu/", "AuthenticatedView.html"));
            }
            else
            {
                loginView = File.ReadAllText(string.Concat(FileRelativePath, "Views/", "Menu/", "UnAuthenticatedView.html"));
            }

            layout = layout.Replace("@IsAuthenticatedView", loginView);

            response.Content = Encoding.UTF8.GetBytes(layout);

            return response;
        }
    }
}
