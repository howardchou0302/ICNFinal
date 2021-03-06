﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

enum PROGRESS{
    water = 1,
    metal,
    coal,
    total
}

enum CharacterStats
{
    Idle = 1,
    Walk,
    Coal,
    Metal,
    Water,
    Bomb
}

namespace GameServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static Dictionary<int, float> progress;
        public static Dictionary<int, Projectile> projectileList;
        public static Dictionary<int, Bomb> bombList;
        public static int nextProjectile;
        public static int nextBomb;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        /// <summary>Starts the server.</summary>
        /// <param name="_maxPlayers">The maximum players that can be connected simultaneously.</param>
        /// <param name="_port">The port to start the server on.</param>
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on port {Port}.");
        }

        public static void SetPlayerPos(int i, Vector3 j)
        {
            clients[i].player.position = j;
            ServerSend.PlayerPosition(clients[i].player);
        }

        public static void SetGunRotation(int i, Quaternion j)
        {
            clients[i].player.gunRotation = j;
            ServerSend.GunRotation(clients[i].player);
        }

        public static void PlayerPickItem(int i, int j)
        {
            if(j == (int)PROGRESS.water) clients[i].player.state = CharacterStats.Water;
            else if(j == (int)PROGRESS.metal) clients[i].player.state = CharacterStats.Metal;
            else if(j == (int)PROGRESS.coal)  clients[i].player.state = CharacterStats.Coal;
            else Console.WriteLine("[Server.cs/PlayerPickItem] ITEM ERROR.");
            ServerSend.PlayerWithItem(clients[i].player);
        }

        public static void PlayerPlaceItem(int i)
        {
            if(clients[i].player.state == CharacterStats.Water)
            {
                if(progress[(int)PROGRESS.water] + 0.1F >= 1) progress[(int)PROGRESS.water] = 1;
                else progress[(int)PROGRESS.water] += 0.1F;
            }
            else if(clients[i].player.state == CharacterStats.Coal)
            {
                if(progress[(int)PROGRESS.coal] + 0.1F >= 1) progress[(int)PROGRESS.coal] = 1;
                else progress[(int)PROGRESS.coal] += 0.1F;
            }
            else if(clients[i].player.state == CharacterStats.Metal)
            {
                if(progress[(int)PROGRESS.metal] + 0.1F >= 1) progress[(int)PROGRESS.metal] = 1;
                else progress[(int)PROGRESS.metal] += 0.1F;
            }
            clients[i].player.state = CharacterStats.Idle;
            ServerSend.PlayerDropItem(clients[i].player);
            ServerSend.GlobalProgress(progress);
        }

        public static void AddProjectile(Vector3 j, Quaternion k)
        {
            int i = 0;
            for(i = nextProjectile; projectileList[i%200] != null; ++i) ;
            projectileList.Add(i, new Projectile(i, j, k));
            ServerSend.SpawnProjectile(projectileList[i]);
        }

        public static void AddBomb(Vector3 j)
        {
            int i = 0;
            for(i = nextBomb; bombList[i%200] != null; ++i) ;
            bombList.Add(i, new Bomb(i, j));
            ServerSend.SpawnBomb(bombList[i]);
        }

        public static void ProjectileExploded(int i, int j)
        {
            if(projectileList[j] == null)
            {
                Console.WriteLine("[Server.cs/ProjectileExploded] projectile not found.");
                return;
            }
            else projectileList[j] = null;
            clients[i].player.state = CharacterStats.Bomb;
            ServerSend.PlayerFrozen(clients[i].player);
            ServerSend.ProjectileExploded(j);
            ServerSend.PlayerDropItem(clients[i].player);
        }

        public static void BombExploded(int i, int j)
        {
            if(bombList[j] == null)
            {
                Console.WriteLine("[Server.cs/BombExploded] bomb not found.");
                return;
            }
            else bombList[j] = null;
            clients[i].player.state = CharacterStats.Bomb;
            ServerSend.PlayerFrozen(clients[i].player);
            ServerSend.BombExploded(j);
            ServerSend.PlayerDropItem(clients[i].player);
        }

        public static void PlayerEliminated(int i){
            ServerSend.PlayerEliminated(i);
        }

        /// <summary>Handles new TCP connections.</summary>
        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        /// <summary>Receives incoming UDP data.</summary>
        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0)
                    {
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null)
                    {
                        // If this is a new connection
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        // Ensures that the client is not being impersonated by another by sending a false clientID
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
        /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
        /// <param name="_packet">The packet to send.</param>
        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }

        /// <summary>Initializes all necessary server data.</summary>
        private static void InitializeServerData()
        {    
            progress = new Dictionary<int, float>()
            {
                {(int)PROGRESS.water, 0},
                {(int)PROGRESS.metal, 0},
                {(int)PROGRESS.coal, 0},
                {(int)PROGRESS.total, 0}
            };

            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                { (int)ClientPackets.playerGunDirection, ServerHandle.PlayerGunDirection },
                { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot },
                { (int)ClientPackets.playerPickItem, ServerHandle.PlayerPickItem },
                { (int)ClientPackets.playerPlaceItem, ServerHandle.PlayerPlaceItem },
                { (int)ClientPackets.playerPlaceBomb, ServerHandle.PlayerPlaceBomb },
                { (int)ClientPackets.projectileExploded, ServerHandle.ProjectileExploded },
                { (int)ClientPackets.bombExploded, ServerHandle.BombExploded },
            };
            Console.WriteLine("Initialized packets.");

            nextProjectile = 0;
            nextBomb = 0;
        }
    }
}
