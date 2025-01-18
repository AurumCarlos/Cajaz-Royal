using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private Movements movement;
    [SerializeField] private Playermouv Playermouv;
    [SerializeField] private GameObject camera;
    [SerializeField] private gun gun;
    [SerializeField] private Health health;
    

    public void IsLocalplayer()
    {
        movement.enabled = false;
        Playermouv.enabled = true;
        gun.enabled = true;
        health.enabled = true;
        camera.SetActive(true);
    }
}
