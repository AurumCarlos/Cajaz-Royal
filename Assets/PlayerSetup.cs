using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private  Movements movement;   
    [SerializeField] private  Playermouv Playermouv;   
    
    [SerializeField] private GameObject camera;

    public void IsLocalplayer()
    {
        movement.enabled = true;
        Playermouv.enabled = true;
        camera.SetActive(true);
    }
}
