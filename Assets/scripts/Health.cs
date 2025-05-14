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
    

    private void Start()
    {
        //healthText.text = health.ToString();
    }
    
    public int Gethealth() => health;
    public bool IsAlive() => isalive;
    

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
            //playercontGameObject.GetComponentInParent<Transform>().position = new Vector2(0,0);
            //playercontGameObject.GetComponent<Transform>().position = new Vector2(0,0);
            //Destroy(playercontGameObject);
        }
    }

    
    // Update is called once per frame
    
    private void Update()
    {
        healthText.text = health.ToString(); 
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
