using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    
    [SerializeField] private Button SERV;
    [SerializeField] private Button HOST;
    [SerializeField] private Button CLIENT;
    // Start is called before the first frame update
    private void Awake()
    {
        SERV.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });        
        HOST.onClick.AddListener(() =>
                      {
                NetworkManager.Singleton.StartHost();
            });        
        CLIENT.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            });
    }

    // Update is called once per frame

}
