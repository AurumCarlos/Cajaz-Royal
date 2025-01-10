using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public Movements movement;   
    
    public GameObject camera;

    public void IsLocalplayer()
    {
        movement.enabled = true;
        camera.SetActive(true);
    }
}
