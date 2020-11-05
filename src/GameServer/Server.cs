using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    public class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }

        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        private static TcpListener _tcpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();

            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            Console.WriteLine($"Server started on {Port}.");
        }

        private static void TcpConnectCallback(IAsyncResult result)
        {
            var client = _tcpListener.EndAcceptTcpClient(result);

            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for (var i = 1; i <= MaxPlayers; i++)
            {
                if (Clients[i].Tcp.Socket is null)
                {
                    Clients[i].Tcp.Connect(client);
                    return;
                }
            }

            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void InitializeServerData()
        {
            for (var i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }
        }
    }
}