using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;

public class ClientHandle : MonoBehaviour
{
    public static void welcome(Packet _packet)//welcomeReceived
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        Debug.Log($"Message from server: {_msg}");
        Client.instance.id = _myId;
        ClientSend.WelcomeReceived();// send hello to server, server will have a class named packethandler to catch this
        Debug.Log("Get the welcome message");
        Debug.Log($"{((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port}"); 
        if (timer.instance == null)
            Debug.Log("null");// 之後要用timer連?
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port, Constants.ServerIP, 26950); // connect to the server with udp by sending the local ip
    }

    /*public static void UDPTest(Packet _packet)
    {
        string _msg = _packet.ReadString();

        Debug.Log($"Receive packet via UDP, containing message: {_msg}");
        ClientSend.UDPTestReceived();
    }*/

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);


        //ClientSend.LocalCollection(Vector3.zero);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        if ((_id != Client.instance.id))
        {
            //Debug.Log($"UDP Message from server: {_id} and new position :({ _position.x},{_position.y})");
            try
            {
                Debug.Log($"count:{GameManager.players[_id].transform.childCount}");
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

    public static void PlayerFrozen(Packet _packet) //TODO
    {
        int _id = _packet.ReadInt();
        try
        {
            GameManager.instance.SetFrozen(_id);
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log($"{e}");
            // do nothing, since we have not spawn the client
        }
    }

    public static void PlayerWithItem(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _item = _packet.ReadInt();
        try
        {
            //TODO
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log($"{e}");
            // do nothing, since we have not spawn the client
        }
    }

    public static void PlayerDropItem(Packet _packet)
    {
        int _id = _packet.ReadInt();
        try
        {
            //TODO
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log($"{e}");
            // do nothing, since we have not spawn the client
        }
    }

    public static void GunRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Debug.Log($"_id:{_id}");
        //Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        try
        {
            if(_rotation != Quaternion.identity)
            {
                // 傳給id的人 
            }
            else
            {
                if (true)
                {

                }
            }
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log($"{e}");
            // do nothing, since we have not spawn the client
        }
    }

    public static void SpawnProjectile(Packet _packet)
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

    public static void ProjectileExploded(Packet _packet)
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

    public static void GlobalProgress(Packet _packet)
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

    public static void SpawnBomb(Packet _packet)
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

    public static void BombExploded(Packet _packet)
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
}
