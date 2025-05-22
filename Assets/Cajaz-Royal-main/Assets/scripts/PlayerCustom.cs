using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerCustom : Player
{
    // Start is called before the first frame update
    protected internal PlayerCustom(string nickName, int actorNumber, bool isLocal) : base(nickName, actorNumber, isLocal)
    {
    }

    protected internal PlayerCustom(string nickName, int actorNumber, bool isLocal, Hashtable playerProperties) : base(nickName, actorNumber, isLocal, playerProperties)
    {
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
