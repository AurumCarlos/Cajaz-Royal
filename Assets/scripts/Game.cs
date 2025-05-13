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
    
    private int nb = 0;
    
    [SerializeField]RoomManager roomManager;
    private void Update()
    { 
        GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Player");
        
        
        nb = 0;
        foreach (var VARIABLE in ennemis)
        {

            if (VARIABLE.GetComponentInChildren<Health>().IsAlive())
            {
                nb += 1;
            }
        }
        Debug.Log((nb,ennemis.Length));

        if (nb >= 1)
        {
            SceneManager.LoadScene("Main Menu");
        }
        
        
        
        return;
        

        
        
        if ( Input.GetAxisRaw("Horizontal") != 0) roomManager.GetComponent<PhotonView>().RPC("Reload", RpcTarget.AllBuffered);
        
    }
    
    
}