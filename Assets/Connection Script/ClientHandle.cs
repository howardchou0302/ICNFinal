using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)//welcomeReceived
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        Debug.Log($"Message from server: {_msg}");
        client.instance.id = _myId;
        ClientSend.WelcomeReceived();// send hello to server, server will have a class named packethandler to catch this
        Debug.Log("Get the welcome message");
        Debug.Log($"{((IPEndPoint)client.instance.tcp.socket.Client.LocalEndPoint).Port}"); // 
        timer.instance.ConnectToServerUDP(((IPEndPoint)client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
        ClientSend.LocalCollection(Vector3.zero);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        if ((_id != client.instance.id))
        {
            //Debug.Log($"UDP Message from server: {_id} and new position :({ _position.x},{_position.y})");
            try
            {
                GameObject player = GameManager.players[_id].transform.GetChild(2).gameObject;
                player.transform.position = _position;
            }
            catch (KeyNotFoundException e)
            {
                // do nothing, since we have not spawn the client
                Debug.Log($"{e}");
            }
        }
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();
        try
        {
            GameManager.players[_id].transform.rotation = _rotation;
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log($"{e}");
            // do nothing, since we have not spawn the client
        }
    }

    public static void Global_Progress(Packet _packet)
    {
        //int _teamID = _packet.ReadInt();
        Vector3 _progress = _packet.ReadVector3();
        Debug.Log($"Global collection from server : ({ _progress.x}, {_progress.y}, {_progress.z})");
        try
        {
            //if (client.instance.id % 2 == _teamID)
            //{
            Constants.globalCollection.X = _progress.x;
            Constants.globalCollection.Y = _progress.y;
            Constants.globalCollection.Z = _progress.z;
            
            //} 
            //ProgressBar.transform.GetChild(2).transform.GetChild(2).text
        }
        catch (KeyNotFoundException e)
        {
            // do nothing, since we have not spawn the client
            Debug.Log($"{e}");
        }
    }

}
