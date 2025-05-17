using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Health : MonoBehaviourPun
{
    [SerializeField] private GameObject playercontGameObject;
    public bool islocalplayer;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI healthText;

    private bool isalive = true;
    [SerializeField] private int health;
    [SerializeField] private Transform pos;

    private float time;

    
    

    
    public int Gethealth() => health;
    public bool IsAlive() => isalive;


    private void Start()
    {
        
    }

    [PunRPC]
    public void damage(int damage)
    {
        health -= damage;
        if (health <= 0 && isalive)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_Die", RpcTarget.All);
            }
        }
    }

    
    // Update is called once per frame

    private void Update()
    {
        time -= Time.deltaTime;
        healthText.text = health.ToString();

        if (DamageCircle.IsOutsideCircle_Static(pos.position) && time <= 0)
        {
            damage(1);
            time = 1;
        }
    }

    [PunRPC]
    void RPC_Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SortingGroup>().sortingOrder = -10;
        GetComponentInChildren<gun>().enabled = false;
        isalive = false;
        // désactiver contrôles, animations, etc.
    }
    
    
}
