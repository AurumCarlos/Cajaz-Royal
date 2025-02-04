using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    //[SerializeField] private Movements movement;
    [SerializeField] private Playermouv Playermouv;
    [SerializeField] private GameObject camera;
    [SerializeField] private gun gun;
    [SerializeField] private Health health;

    private string Nick;
    [SerializeField] private TextMeshPro Nickname;
    

    public void IsLocalplayer()
    {
        //movement.enabled = false;
        Playermouv.enabled = true;
        gun.enabled = true;
        health.enabled = true;
        camera.SetActive(true);
        health.islocalplayer = true;
        Nickname.enabled = true;
    }

    [PunRPC]
    public void setnick(string nick)
    {
        Nick = nick;
        
        Nickname.text = Nick;
    }
}
