﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            Server.clients[_fromClient].SendIntoGame(_username);
        }

        public static void PlayerMovement(int _fromClient, Packet _packet)
        { Server.SetPlayerPos(_fromClient, _packet.ReadVector3()); }

        // playerGunDirection,
        public static void PlayerGunDirection(int _fromClient, Packet _packet)
        { Server.SetGunRotation(_fromClient, _packet.ReadQuaternion()); }

        // playerShoot,
        public static void PlayerShoot(int _fromClient, Packet _packet)
        { Server.AddProjectile(_packet.ReadVector3(), _packet.ReadQuaternion()); }

        // playerPickItem,
        public static void PlayerPickItem(int _fromClient, Packet _packet)
        { Server.PlayerPickItem(_fromClient, _packet.ReadInt()); }

        // playerPlaceItem,
        public static void PlayerPlaceItem(int _fromClient, Packet _packet)
        { Server.PlayerPlaceItem(_fromClient); }

        // playerPlaceBomb,
        public static void PlayerPlaceBomb(int _fromClient, Packet _packet)
        { Server.AddBomb(_packet.ReadVector3()); }

        // projectileExploded
        public static void ProjectileExploded(int _fromClient, Packet _packet)
        { Server.ProjectileExploded(_fromClient, _packet.ReadInt()); }
        // bombExploded
        public static void BombExploded(int _fromClient, Packet _packet)
        { Server.BombExploded(_fromClient, _packet.ReadInt()); }

    }
}
