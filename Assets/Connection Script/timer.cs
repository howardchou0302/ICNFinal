using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timer : MonoBehaviour
{
    public static timer instance;
    public GameObject canvus;
    // Start is called before the first frame update
    public void Start()
    {
        ConnectToServer();
    }

    // Update is called once per frame

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    public void ConnectToServer()
    {
        canvus.SetActive(false);
        Client.instance.ConnectToServer(Constants.ServerIP, 26950);
        //Client.instance.ConnectToServer();
        Debug.Log("test for tcp");
    }

    public void ConnectToServerUDP(int _local)
    {
        Client.instance.udp.Connect(_local, Constants.ServerIP, 26950); 
        Debug.Log("test for connect to server with UDP");
    }
}
