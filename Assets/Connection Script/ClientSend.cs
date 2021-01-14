using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.id);
            _packet.Write("hello");

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(Vector3 _input)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {

            _packet.Write(_input);
            if (Client.instance.isConnected)
            {

                SendUDPData(_packet);
            }
        }
    }

    /*          { (int)ClientPackets.playerGunDirection, ServerHandle.PlayerGunDirection },
                { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot },
                { (int)ClientPackets.playerPickItem, ServerHandle.PlayerPickItem },
                { (int)ClientPackets.playerPlaceItem, ServerHandle.PlayerPlaceItem },
                { (int)ClientPackets.playerPlaceBomb, ServerHandle.PlayerPlaceBomb },
                { (int)ClientPackets.projectileExploded, ServerHandle.ProjectileExploded },
                { (int)ClientPackets.bombExploded, ServerHandle.BombExploded },
                mining
     */
    
    public static void BombExploded(int BombId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.bombExploded))
        {
            _packet.Write(BombId);
            SendTCPData(_packet);
        }
    }

    public static void ProjectileExploded(int ProjectileId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.projectileExploded))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(ProjectileId);
            SendTCPData(_packet);
        }
    }

    public static void PlayerGunDirection(Quaternion _input)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerGunDirection))
        {
            _packet.Write(_input);
            if (Client.instance.isConnected && Client.instance.id%2 == 0)
            {

                SendUDPData(_packet);
            }
        }
    }
    public static void PlayerShoot(Vector3 _inputVector, Quaternion _inputQ)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(_inputVector);
            _packet.Write(_inputQ);
            SendTCPData(_packet);
        }
    }
    public static void PlayerPickItem(int _item)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerPickItem))
        {
            //_packet.Write(Client.instance.id);
            _packet.Write(_item);
            SendTCPData(_packet);
        }
    }

    public static void PlayerPlaceBomb()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerPlaceBomb))
        {
            _packet.Write(Client.instance.id);
            SendTCPData(_packet);
        }
    }

    public static void PlayerPlaceItem(int _item)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerPlaceItem))
        {
            _packet.Write(_item);
            SendTCPData(_packet);
        }
    }

    #endregion

}
