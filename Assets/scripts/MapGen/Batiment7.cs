using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Batiment7 : MonoBehaviour
{

    [SerializeField] private GameObject Ext;
    // Start is called before the first frame update
    void Start()
    {
        Ext.SetActive(true);
    }

    
    // Update is called once per frame
    void Update()
    {
        //Ext.SetActive(true);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (Ext != null)
        {
            Health t = other.gameObject.GetComponent<Health>();
            Playermouv p = other.gameObject.GetComponent<Playermouv>();
            if (p!= null && !PhotonNetwork.IsConnected || t != null && (t.islocalplayer))
            {
                Ext.SetActive(false);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        if (Ext != null)
        {
            Health t = other.gameObject.GetComponent<Health>();
            if (!PhotonNetwork.IsConnected || t != null && (t.islocalplayer))
            {
                Ext.SetActive(true);
            }
        }
    }
}
