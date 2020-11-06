using System;
using System.Net.Sockets;

namespace GameServer
{
    public class Client
    {
        public static int DataBufferSize => 4096;
        public int Id { get; }
        public TCP Tcp { get; }

        public Client(int clientId)
        {
            Id = clientId;
            Tcp = new TCP(clientId);
        }

        public class TCP
        {
            public TcpClient Socket { get; private set; }

            private readonly int _id;
            private NetworkStream _stream;
            private byte[] _receiveBuffer;

            public TCP(int id) 
                => _id = id;

            public void Connect(TcpClient socket)
            {
                Socket = socket;
                socket.ReceiveBufferSize = DataBufferSize;
                socket.SendBufferSize = DataBufferSize;

                _stream = socket.GetStream();

                _receiveBuffer = new byte[DataBufferSize];

                _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

                // TODO: send welcome packet
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    var byteLength = _stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        // TODO: Disconnect the client
                        return;
                    }

                    var data = new byte[byteLength];
                    Array.Copy(_receiveBuffer, data, byteLength);

                    // TODO: Handle data
                    _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving TCP data: {ex}");
                    // TODO: Disconnect the client
                }
            }
        }
    }
}