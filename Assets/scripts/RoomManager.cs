using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;
    public GameObject player;
    [Space]
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject UIname;
    [SerializeField] private TMP_InputField UIroom;
    [SerializeField] private GameObject UIconnecting;
    [SerializeField] private GameObject Map;
    [SerializeField] private GameObject Game;
    private WalkerGenerator m;
    private string Nick;
    private string Room = "test";
    private GameObject _player;
    
    private void Awake()
    {
        Instance = this;
    }




    public void changenick(string nick)
    {
        Nick = nick;
    }

    public void joinbuton()
    {

        Room = UIroom.text;
        if (string.IsNullOrEmpty(Room)) Room = "test";
        PhotonNetwork.ConnectUsingSettings();

        UIname.SetActive(false);
        UIconnecting.SetActive(true);
        Debug.Log(Room);
        
        //Spwan();
    }
    void Start()
    {
        Debug.Log("Connection...");
        //PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        
        Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom(Room, null, null);
        Debug.Log("Joined lobby");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        Debug.Log("Joined room");
        camera.SetActive(false);
        
        Game.SetActive(true);
        Spwan();
        
    }

    public void Spwan()
    {
        (int a, int b) = WalkerGenerator.FindSpawn();
    
        if (a == -1 && b == -1)
        {
            Debug.Log("No spawn found");
            return;
        }
        Debug.Log((a,b));
        
        _player = PhotonNetwork.Instantiate(player.name, new Vector3(a,b,0), Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalplayer();
        _player.GetComponent<PhotonView>().RPC("setnick", RpcTarget.AllBuffered, Nick);
        int Seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond%10000000);
        //Map.GetComponent<PhotonView>().RPC("InitializeGrid", RpcTarget.AllBuffered, Seed);
        
    }
    [PunRPC]
    public void Reload()
    {
        if (_player != null)
        {
            Destroy(_player);
        }
        Debug.Log("Reload game");
        
        Spwan();
    }
    
    
}