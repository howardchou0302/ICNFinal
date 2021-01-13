using System;
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
        { Server.clients[_fromClient].player.SetPos(_packet.ReadVector3()); }

        // playerGunDirection,
        public static void PlayerGunDirection(int _fromClient, Packet _packet)
        { Server.clients[_fromClient].player.SetGunRotation(_packet.ReadQuaternion()); }

        // playerShoot,
        public static void PlayerShoot(int _fromClient, Packet _packet)
        { Server.AddProjectile(_packet.ReadVector3(), _packet.ReadQuaternion()); }

        // playerPickItem,
        public static void PlayerPickItem(int _fromClient, Packet _packet)
        {

        }

        // playerPlaceItem,
        public static void PlayerPlaceItem(int _fromClient, Packet _packet)
        {

        }

        // playerPlaceBomb,
        public static void PlayerPlaceBomb(int _fromClient, Packet _packet)
        {

        }

        // projectileExploded
        public static void ProjectileExploded(int _fromClient, Packet _packet)
        {

        }
        // bombExploded
        public static void BombExploded(int _fromClient, Packet _packet)
        {

        }

    }
}
