using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        //Debug.Log($"ID is : {client.instance.id}");
        GameObject player = GameManager.players[client.instance.id].transform.GetChild(2).gameObject;
        Vector3 _input = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

        ClientSend.PlayerMovement(_input);
    }
}
