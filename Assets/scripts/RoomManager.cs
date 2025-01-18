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
    public Transform SpawnPoint;
    [SerializeField] private GameObject camera;
    // Start is called before the first frame update
    
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Debug.Log("Connection...");
        PhotonNetwork.ConnectUsingSettings();
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


    }
    
    
}
