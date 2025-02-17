using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;
    public GameObject player;
    [Space]
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject UIname;
    [SerializeField] private GameObject UIconnecting;
    [SerializeField] private GameObject Map;
    private string Nick;
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

        PhotonNetwork.ConnectUsingSettings();

        UIname.SetActive(false);
        UIconnecting.SetActive(true);

        
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
        PhotonNetwork.JoinOrCreateRoom("test", null, null);
        Debug.Log("Joined lobby");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        Debug.Log("Joined room");
        camera.SetActive(false);
        
        Spwan();
    }

    public void Spwan()
    {
        GameObject _player = PhotonNetwork.Instantiate(player.name, SpawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalplayer();
        _player.GetComponent<PhotonView>().RPC("setnick", RpcTarget.AllBuffered, Nick);
        int Seed = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond%10000000);
        Map.GetComponent<PhotonView>().RPC("InitializeGrid", RpcTarget.AllBuffered, Seed);
    }
    
    
}












