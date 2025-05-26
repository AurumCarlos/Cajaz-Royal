using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnableSoloMode : MonoBehaviourPun
{
    void Start()
    {
        Debug.Log(PhotonNetwork.IsConnected);
        transform.GetComponent<SpawnItems>().gameObject.SetActive(PhotonNetwork.IsConnected);
        transform.GetComponent<SpawnItemsSolo>().gameObject.SetActive(!PhotonNetwork.IsConnected);

        for (int i = 0; i < 7; i++)
        { 
            //transform.GetChild(i).gameObject.SetActive(true);
        }    
    }
}