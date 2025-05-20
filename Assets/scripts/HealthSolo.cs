using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HealthSolo : MonoBehaviourPun
{
    [SerializeField] private GameObject playercontGameObject;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI healthText;
    private bool isalive = true;
    [SerializeField] private int health;
    

    
    

    
    public int Gethealth() => health;
    public bool IsAlive() => isalive;


    private void Start()
    {
        
    }
    
    public void damage(int damage)
    {
        health -= damage;
        if (health <= 0 && isalive)
        {
            Die();
        }
    }

    
    // Update is called once per frame

    private void Update()
    {

    }


    void Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SortingGroup>().sortingOrder = -10;
        GetComponentInChildren<Gun>().enabled = false;
        isalive = false;
        // désactiver contrôles, animations, etc.
    }
    
    
}
