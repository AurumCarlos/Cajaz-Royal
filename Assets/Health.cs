using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] private int health;

    private void Start()
    {
        //healthText.text = health.ToString();
    }
    [PunRPC]
    public void damage(int damage)
    {
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }

    // Update is called once per frame
    
    private void Update()
    {
        healthText.text = health.ToString(); 
    }
}
