using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game : MonoBehaviourPunCallbacks
{
    private bool IsEnd = false;
    private int nb = 0;
    
    [SerializeField] GameObject EndGameUI;
    
    [SerializeField]RoomManager roomManager;
    
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    private void Update()
    { 
        //GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Player");
        
        
        nb = 0;
        //foreach (var VARIABLE in ennemis)
        //{

         //   if (VARIABLE.GetComponentInChildren<Health>().IsAlive())
            //{
           //     nb += 1;
           // }
       // }
        if (Input.GetKeyDown(KeyCode.Space)) IsEnd = true;
        if (IsEnd)
        {
            ToggleHud(false);
            photonView.RPC("EndMenu", RpcTarget.All);
            //IsEnd = false;
        }
            
        
        
        
        
        //if ( Input.GetAxisRaw("Horizontal") != 0) roomManager.GetComponent<PhotonView>().RPC("Reload", RpcTarget.AllBuffered);
        
    }

    
    private void ToggleHud(bool b)
    {
        GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in ennemis)
        {
            Transform t = player.transform.Find("Canvas");
            if (t)
            {
                t.gameObject.SetActive(b);
            }

        }
    }

    [PunRPC]
    private void EndMenu()
    {
        //EndGameUI.GetComponent<MenuEndGame>().ShowEndGame();
        EndGameUI.SetActive(true);
    }


}