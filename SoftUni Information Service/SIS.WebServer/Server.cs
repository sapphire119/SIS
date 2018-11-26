﻿using SIS.WebServer.Routing;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SIS.WebServer
{
    public class Server
    {
        private const string LocalHostIpAdress = "127.0.0.1";

        private readonly int port;

        private readonly TcpListener listener;

        private readonly ServerRoutingTable serverRoutingTable;

        private bool isRunning;

        public Server(int port, ServerRoutingTable serverRoutingTable)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalHostIpAdress), port);

            this.serverRoutingTable = serverRoutingTable;
        }

        public void Run()
        {
            this.listener.Start();
            this.isRunning = true;

            Console.WriteLine($"Server started at http://{LocalHostIpAdress}:{this.port}");
            while (this.isRunning)
            {
                Console.WriteLine("Waiting for client...");

                var client = this.listener.AcceptSocketAsync().GetAwaiter().GetResult();

                Task.Run(() => this.ListenLoopAsync(client));
            }
        }

        public async void ListenLoopAsync(Socket client)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var connectionHandler = new ConnectionHandler(client, this.serverRoutingTable);
            await connectionHandler.ProcessRequestAsync();
        }
    }
}
