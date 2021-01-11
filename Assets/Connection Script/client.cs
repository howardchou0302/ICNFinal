using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;
    // 助教的code沒有以下這段//ip is get in at start in canvus change"
    //public readonly string ipaddress = "127.0.0.1"; 
     // 10.118.163.7
    public int port = 26950;
    // ================= //
    public int id = 0;
    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;
    // why??? what is delegate void?? => sound like pointer
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("make instance");
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        Debug.Log("Awake...");
    }

    private void Start()
    {
        Debug.Log("Start");
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
    // here get the ip and port
    // public void ConnectToServer(String ipaddress, int port)
    public void ConnectToServer(string ipaddress, int port)
    {
        InitializeClientData();
        Debug.Log("at connecting...");
        isConnected = true;
        Debug.Log($"{ipaddress},{port}");
        tcp.Connect(ipaddress, port);
        //udp.Connect(ipaddress, port);

    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        // this place call the ip addr. and port, but who?? ->ConnectToServer
        public void Connect(String ipaddress, int port)
        {
            Debug.Log($"{ipaddress},{port}");
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            if (socket == null)
                Debug.Log("fuckyou");

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(IPAddress.Parse(ipaddress), port, ConnectCallback, socket);
            
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                Debug.Log("not connecting");
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }


        private void ReceiveCallback(IAsyncResult _result)
        {
            Debug.Log("TCPReceiveCallback");
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }
        private bool HandleData(byte[] _data)
        {
            Debug.Log("TCPHandleData");
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }
            Debug.Log($"_packetLength:{_packetLength}");
            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                Debug.Log($"in while, read packet");
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Debug.Log($"_packetId:{_packetId}");
                        packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            Debug.Log("TCPDisconnect");
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        // no constructor, endpoint write in the connect 
        public void Connect(int _localPort, String ipaddress, int port)
        {

            endPoint = new IPEndPoint(IPAddress.Parse(ipaddress), port);
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.id);// cuz all udp at clients will serve at the same udp at server, so use id to identify
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        private void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerFrozen, ClientHandle.PlayerFrozen },
            { (int)ServerPackets.playerWithItem, ClientHandle.PlayerWithItem },
            { (int)ServerPackets.playerDropItem, ClientHandle.PlayerDropItem },
            { (int)ServerPackets.globalProgress, ClientHandle.GlobalProgress },
            { (int)ServerPackets.gunRotation, ClientHandle.GunRotation },
            { (int)ServerPackets.spawnProjectile, ClientHandle.SpawnProjectile },
            { (int)ServerPackets.projectileExploded, ClientHandle.ProjectileExploded },
            { (int)ServerPackets.spawnBomb, ClientHandle.SpawnBomb },
            { (int)ServerPackets.bombExploded, ClientHandle.BombExploded }
        };
        Debug.Log("Initialized packets.");
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server.");
        }
    }
}
