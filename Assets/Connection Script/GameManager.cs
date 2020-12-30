using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab_C;
    public GameObject localPlayerPrefab_I;
    public GameObject playerPrefab_C;
    public GameObject playerPrefab_I;


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

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == client.instance.id)
        {
            if(_id % 2 == 0)
            {
                _player = Instantiate(localPlayerPrefab_C, _position, _rotation);
            } else
            {
                _player = Instantiate(localPlayerPrefab_I, _position, _rotation);
            }
        }
        else
        {
            if (_id % 2 == 0)
            {
                _player = Instantiate(playerPrefab_C, _position, _rotation);
            }
            else
            {
                _player = Instantiate(playerPrefab_I, _position, _rotation);
            }
        }
        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }
}
