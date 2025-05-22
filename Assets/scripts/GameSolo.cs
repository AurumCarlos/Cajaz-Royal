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


public class GameSolo : MonoBehaviourPunCallbacks
{
    private bool IsEnd = false;
    [SerializeField] GameObject EndGameUI;
    private float time = 5;
    [SerializeField] private GameObject Playercont;
    [SerializeField] private GameObject Player;
    [SerializeField] private WalkerGenerator W;

    void Start()
    {
    
        Debug.Log(DateTime.Now.Millisecond);
        
        W.launch((int)(DateTime.Now.Millisecond));
    }
    private void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Player");

            int nb = 0;

            foreach (var player in ennemis)
            {
                if (player.GetComponentInChildren<Health>().IsAlive()) nb += 1;
            }
            
            if (nb <= 0) 
            {
                ToggleHud(false);
                EndMenu();
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
    
    private void EndMenu()
    {
        EndGameUI.SetActive(true);
    }


}