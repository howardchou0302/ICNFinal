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
            /*if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Receive \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }*/
            Server.clients[_fromClient].SendIntoGame(_username);
        }

        public static void PlayerMovement(int _fromClient, Packet _packet)
        {
            //int _id = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();
            Vector3 _rotation = _packet.ReadVector3();
            //Console.WriteLine($"Player{_fromClient}: X:{_rotation.X},Y:{_rotation.Y},{_rotation.Z}");
            Server.clients[_fromClient].player.SetInput(_position);
            /*bool[] _inputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = _packet.ReadBool();
            }
            Quaternion _rotation = _packet.ReadQuaternion();

            Server.clients[_fromClient].player.SetInput(_inputs, _rotation);*/
        }

        public static void LocalCollection(int _fromClient, Packet _packet)
        {
            //TODO
        }
    }
}
