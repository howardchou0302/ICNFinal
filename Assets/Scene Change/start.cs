using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; //Button
using UnityEngine.SceneManagement; //SceneManager
using UnityEngine;
#if Unity_Editor
using UnityEditor;
#endif

public class start : MonoBehaviour
{   
    //public static client client;
    public int sceneIndex = 1; //要載入的Scene
    public static start instance;
    public InputField IPaddress;
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

    public void ClickEvent()
    {
        //切換Scene
        Debug.Log("ClickEvent");
        SceneManager.LoadScene (sceneIndex);
        Debug.Log(IPaddress.text);
        Client.instance.ConnectToServer(IPaddress.text , 26950);
        Constants.ServerIP = IPaddress.text;
    }
    
}

