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
    private bool IsStart = false;
    private int nb = 0;
    
    [SerializeField] GameObject EndGameUI;
    [SerializeField] WalkerGenerator walkerGenerator;
    
    private int seed;
    [SerializeField] GameObject Zone;
    
    [SerializeField]RoomManager roomManager;
    private PhotonView photonView;

    private float time = 5;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        
    }

    void Start()
    {
        if (!IsStart)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }


    private void Update()
    {

        time -= Time.deltaTime;
        if (time <= 0)
        {
            GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Player");

            int nb = 0;

            foreach (var VARIABLE in ennemis)
            {
                if (VARIABLE.GetComponentInChildren<Health>().IsAlive()) nb += 1;
            }

            if (nb <= 2) 
            {
                ToggleHud(false);
                photonView.RPC("EndMenu", RpcTarget.All);
            }
        }
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
        EndGameUI.SetActive(true);
    }

    [PunRPC]
    private void StartGame()
    {   
        IsStart = true;
        //walkerGenerator.launch(DateTime.Now.Millisecond);
    }





}