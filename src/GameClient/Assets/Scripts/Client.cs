using System;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }
    public static int DataBufferSize => 4096;
    public TCP Tcp;

    [SerializeField]
    private string _ip = "127.0.0.1";

    [SerializeField] 
    private int _port = 26950;

    [SerializeField]
    private int _myId = 0;


    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        Tcp = new TCP();
    }

    public void ConnectToServer()
    {
        Tcp.Connect();
    }

    public class TCP
    {
        public TcpClient Socket { get; private set; }

        private NetworkStream _stream;
        private byte[] _receiveBuffer;

        public void Connect()
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };
            
            _receiveBuffer = new byte[DataBufferSize];

            Socket.BeginConnect(Instance._ip, Instance._port, ConnectCallback, Socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            Socket.EndConnect(result);

            if (!Socket.Connected)
                return;

            _stream = Socket.GetStream();

            _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
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
