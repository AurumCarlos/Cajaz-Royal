#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Health : MonoBehaviourPun
{
    [SerializeField] private GameObject playercontGameObject;
    public bool islocalplayer;
    
    private float _time;
    
    public bool IsAlive { get; private set; }
    
    [Header("UI")]
    [SerializeField] private UI_Health healthBar;
    
    private int _maxHealth = 100;
    private int _plateAbsorption = 20;
    [SerializeField] private int health;
    
    
    private InventoryHandler _inventoryHandler;

    private void Awake()
    {
        IsAlive = true;
        _inventoryHandler = transform.GetComponent<InventoryHandler>();
    }

    private float GetDamageModifiers()
    {
        float damageModifier = 0f;
        for (int i = 0; i < 3; i++)
        {
            Item? item = _inventoryHandler.Inventory[i];
            if (item != null) damageModifier += item.Attributes["defenseModifier"];
        }
        
        return (1-damageModifier)*0.5f;
    }

    [PunRPC]
    public void Damage(int damage)
    {
        try
        {
            if (damage >= 0)
            {
                if (_inventoryHandler.Inventory[1] != null)
                {
                    Item armor = _inventoryHandler.Inventory[1];
                    while (armor.Save != null && armor.Save["Plates"] != 0 && damage >= _plateAbsorption)
                    {
                        armor.Save["Plates"]--;
                        damage -= _plateAbsorption;
                    }
                }
            
                health -= (int)(damage * GetDamageModifiers());
                if (health <= 0 && IsAlive)
                {
                    if (photonView.IsMine)
                    {
                        RPC_Die();
                        //photonView.RPC("RPC_Die", RpcTarget.All);
                    }
                }
            }
            else
            {
                health -= damage;
                if (health >= _maxHealth) health = _maxHealth;
            }
        
            healthBar.Health = health;
        } catch (NullReferenceException) { }
        
    }
    
    private void Update()
    {
        _time -= Time.deltaTime;
        if (DamageCircle.IsOutsideCircle_Static(transform.position) && _time <= 0)
        {

            health--;
            healthBar.Health = health;
            if (health <= 0 && IsAlive)
            {
                if (photonView.IsMine) photonView.RPC("RPC_Die", RpcTarget.All);
            }
            _time = 1;
        }
        try
        {
           
        }
        catch (Exception) {}
        
    }
    
    [PunRPC]
    public void SpawnWorldItemtr(Vector3 position, int itemId, int amount)
    {
        Item item = ItemLoader.GetItem(itemId, amount);
        if (item != null)
        {
            object[] data = { item.ID, item.Amount };
            GameObject go = PhotonNetwork.Instantiate("WorldItem", position, Quaternion.identity, 0, data);
            WorldItem worldItem = go.GetComponent<WorldItem>();
            if (worldItem != null && worldItem.photonView.IsMine)
            {
                // Initialisation locale, les autres clients feront OnPhotonInstantiate
                worldItem.Item = ItemLoader.GetItem(item.ID, item.Amount);
            }
            //_tmp = go.GetComponent<WorldItem>();
            //worldItem.Item = ItemLoader.GetItem(itemId, amount);
            //photonView.RPC("SetWorldItem", RpcTarget.All, itemId, amount);
        }
    }

    private void DropAll() 
    {
        foreach (var item in _inventoryHandler.Inventory)
        {
            if (item != null) 
            {
                Vector3 dir = Utils.GetRandomDir();
                SpawnWorldItemtr(transform.position + dir, item.ID, item.Amount);
            }

        }
    }



    [PunRPC]
    private void DeactivateToAll(int viewID) 
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) 
        {
            if (photonView.ViewID == viewID)
            {
                GetComponentInChildren<SortingGroup>().sortingOrder = -100;
                //player.Find("").SetActive(false);
            }
        }
    }

    [PunRPC]
    void RPC_Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        DropAll();
        photonView.RPC("DeactivateToAll", RpcTarget.All, photonView.ViewID);
        GetComponent<SortingGroup>().sortingOrder = -10;
        GetComponentInChildren<Gun>().enabled = false;

        IsAlive = false;
        // désactiver contrôles, animations, etc.
    }


    
}
