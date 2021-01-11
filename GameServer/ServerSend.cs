using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    class ServerSend
    {
        /// <summary>Sends a packet to a client via TCP.</summary>
        /// <param name="_toClient">The client to send the packet the packet to.</param>
        /// <param name="_packet">The packet to send to the client.</param>
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        /// <summary>Sends a packet to a client via UDP.</summary>
        /// <param name="_toClient">The client to send the packet the packet to.</param>
        /// <param name="_packet">The packet to send to the client.</param>
        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);
        }

        /// <summary>Sends a packet to all clients via TCP.</summary>
        /// <param name="_packet">The packet to send.</param>
        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        /// <summary>Sends a packet to all clients except one via TCP.</summary>
        /// <param name="_exceptClient">The client to NOT send the data to.</param>
        /// <param name="_packet">The packet to send.</param>
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        /// <summary>Sends a packet to all clients via UDP.</summary>
        /// <param name="_packet">The packet to send.</param>
        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
        /// <summary>Sends a packet to all clients except one via UDP.</summary>
        /// <param name="_exceptClient">The client to NOT send the data to.</param>
        /// <param name="_packet">The packet to send.</param>
        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }

        #region Packets
        /// <summary>Sends a welcome message to the given client.</summary>
        /// <param name="_toClient">The client to send the packet to.</param>
        /// <param name="_msg">The message to send.</param>
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        /// <summary>Tells a client to spawn a player.</summary>
        /// <param name="_toClient">The client that should spawn the player.</param>
        /// <param name="_player">The player to spawn.</param>
        public static void SpawnPlayer(int _toClient, Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.username);
                _packet.Write(_player.position);
                _packet.Write(_player.gunRotation);

                SendTCPData(_toClient, _packet);
            }
        }

        /// <summary>Sends a player's updated position to all clients.</summary>
        /// <param name="_player">The player whose position to update.</param>
        public static void PlayerPosition(Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.position);

                SendUDPDataToAll(_packet);
            }
        }

        //playerFrozen
        public static void PlayerFrozen(Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerFrozen))
            {
                _packet.Write(_player.id);

                SendTCPDataToAll(_player.id, _packet);
            }
        }

        // playerWithItem
        public static void PlayerWithItem(Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerWithItem))
            {
                _packet.Write(_player.id);
                _packet.Write((int)_player.state);

                SendTCPDataToAll(_player.id, _packet);
            }
        }

        // playerDropItem
        public static void PlayerDropItem(Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerDropItem))
            {
                _packet.Write(_player.id);

                SendTCPDataToAll(_player.id, _packet);
            }
        }

        // globalProgress
        public static void GlobalProgress(Dictionary<int, float> progress)
        {
            using (Packet _packet = new Packet((int)ServerPackets.globalProgress))
            {
                _packet.Write(progress[(int)PROGRESS.water]);
                _packet.Write(progress[(int)PROGRESS.metal]);
                _packet.Write(progress[(int)PROGRESS.coal]);
                _packet.Write(progress[(int)PROGRESS.total]);

                SendUDPDataToAll(_packet);
            }
        }

        // gunRotation
        public static void GunRotation(Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.gunRotation))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.gunRotation);

                SendUDPDataToAll(_player.id, _packet);
            }
        }
        
        // spawnProjectile
        public static void SpawnProjectile(Projectile _projectile)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnProjectile))
            {
                _packet.Write(_projectile.id);
                _packet.Write(_projectile.position);
                _packet.Write(_projectile.rotation);

                SendTCPDataToAll(_packet);
            }
        }

        // projectileExploded
        public static void ProjectileExploded(Player _player, Projectile _projectile)
        {
            using (Packet _packet = new Packet((int)ServerPackets.projectileExploded))
            {
                _packet.Write(_player.id);
                _packet.Write(_projectile.id);

                SendTCPDataToAll(_packet);
            }
        }

        // spawnBomb
        public static void SpawnBomb(Bomb _bomb)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnBomb))
            {
                _packet.Write(_bomb.id);
                _packet.Write(_bomb.position);

                SendTCPDataToAll(_packet);
            }
        }

        // bombExploded
        public static void BombExploded(Player _player, Bomb _bomb)
        {
            using (Packet _packet = new Packet((int)ServerPackets.bombExploded))
            {
                _packet.Write(_player.id);
                _packet.Write(_bomb.id);

                SendTCPDataToAll(_packet);
            }
        }
        #endregion
    }
}
