using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UI_Health : MonoBehaviourPunCallbacks
{
    public float Health 
    {
        set => _healthBar.localScale = value > 0 ? new Vector3(value / 100, 1, 1) : Vector3.zero;
    }

    private Transform _healthBar;
    private Transform _radiationBar;

    void Start()
    {
        _healthBar = transform.Find("Health_bar");
    }
}
