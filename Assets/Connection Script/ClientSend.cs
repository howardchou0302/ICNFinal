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
                Debug.Log("send rotation");
                //Debug.Log($"Send local player position ({_input.x}, {_input.y})");
                _packet.Write(GameManager.players[Client.instance.id].transform.rotation);

                SendUDPData(_packet);
            }
        }
    }

    public static void LocalCollection(Vector3 _input)
    {
        using (Packet _packet = new Packet((int)ClientPackets.local_collection))
        {
            _packet.Write(_input);
            //Debug.Log($"Send local player collection ({_input.x}, {_input.y}, {_input.z})");
            SendUDPData(_packet);
        }
    }
    #endregion

    /*public static void UDPTestReceived()
    {
        using(Packet _packet = new Packet((int)ClientPackets.udpTestReceived))
        {
            _packet.Write("Received a UDP packet");
            SendUDPData(_packet);
        }
    }*/
}
