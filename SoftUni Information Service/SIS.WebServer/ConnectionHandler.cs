namespace SIS.WebServer
{
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Requests.Intefaces;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Interfaces;
    using SIS.HTTP.Sessions;

    using SIS.WebServer.Api;
    using SIS.WebServer.Results;
    using SIS.WebServer.Routing;

    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class ConnectionHandler
    {
        private readonly Socket client;

        private readonly IHttpHandler handler;

        public ConnectionHandler(Socket client, IHttpHandler handler)
        {
            this.client = client;
            this.handler = handler;
        }

        public async Task ProcessRequestAsync()
        {
            try
            {
                var httpRequest = await this.ReadRequest();

                if (httpRequest != null)
                {
                    string sessionId = this.SetRequestSession(httpRequest);

                    var httpResponse = this.handler.Handle(httpRequest);

                    this.SetResponseSession(httpResponse, sessionId);

                    await this.PrepareResponseAsync(httpResponse);
                }
            }
            catch (BadRequestException e)
            {
                await this.PrepareResponseAsync(new TextResult(e.Message, HttpResponseStatusCode.BadRequest));
            }
            catch(Exception e)
            {
                await this.PrepareResponseAsync(
                    new TextResult(e.Message, HttpResponseStatusCode.InternalServerError));
            }

            this.client.Shutdown(SocketShutdown.Both);
        }

        private async Task PrepareResponseAsync(IHttpResponse httpResponse)
        {
            byte[] byteSegments = httpResponse.GetBytes();

            await this.client.SendAsync(byteSegments, SocketFlags.None);
        }

        private string SetRequestSession(IHttpRequest httpRequest)
        {
            string sessionId = null;

            if (httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = httpRequest.Cookies.GetCookie(HttpSessionStorage.SessionCookieKey);
                sessionId = cookie.Value;
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }

            return sessionId;
        }

        private void SetResponseSession(IHttpResponse response, string sessiondId)
        {
            if (sessiondId != null)
            {
                response
                    .AddCookie(new HttpCookie(HttpSessionStorage.SessionCookieKey,
                        $"{sessiondId}; HttpOnly"));
            }
        }


        private async Task<IHttpRequest> ReadRequest()
        {
            var result = new StringBuilder();
            var data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int numberOfBytesRead = await this.client.ReceiveAsync(data.Array, SocketFlags.None);

                if (numberOfBytesRead == 0) break;

                var bytesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);
                result.Append(bytesAsString);

                if (numberOfBytesRead < 1023) break;
            }

            if (result.Length == 0) return null;

            return new HttpRequest(result.ToString());
        }

        //private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        //{
        //    if (!this.serverRoutingTable.Routes.ContainsKey(httpRequest.RequestMethod)
        //        || !this.serverRoutingTable.Routes[httpRequest.RequestMethod].ContainsKey(httpRequest.Path))
        //    {
        //        return this.ReturnIfResource(httpRequest.Path);
        //    }

        //    return this.serverRoutingTable.Routes[httpRequest.RequestMethod][httpRequest.Path].Invoke(httpRequest);
        //}

        private IHttpResponse ReturnIfResource(string path)
        {
            var extension = path.Substring(path.LastIndexOf('.') + 1);
            
            var root = string.Concat(Directory.GetCurrentDirectory(), "/Resources/", $"{extension}/", path.Substring(path.LastIndexOf('/') + 1));

            if (File.Exists(root))
            {
                var allContent = File.ReadAllBytes(root);

                return new InlineResourceResult(allContent, HttpResponseStatusCode.Ok);
            }

            return new HttpResponse(HttpResponseStatusCode.NotFound);
        }
    }
}
