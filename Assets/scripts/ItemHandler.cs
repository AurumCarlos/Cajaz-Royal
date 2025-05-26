using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemHandler : MonoBehaviourPunCallbacks
{
    public static ItemHandler Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Transform toSpawn;
}
