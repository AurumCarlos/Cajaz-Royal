using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HealthSolo : MonoBehaviour
{
    [SerializeField] private GameObject playercontGameObject;
    [SerializeField] private UI_Health healthBar;

    public bool IsAlive { get; private set; }
    
    private int _maxHealth = 100;
    private int _plateAbsorption = 20;
    
    [SerializeField] private int health;

    private void Update()
    {
        //Debug.Log(health);
    }

    private InventoryHandlerSolo _inventoryHandler;

    private void Awake()
    {
        IsAlive = true;
        _inventoryHandler = transform.GetComponent<InventoryHandlerSolo>();
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


    public void Damage(int damage)
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
            if (health <= 0 && IsAlive) Die();
        }
        else
        {
            health -= damage;
            if (health >= _maxHealth) health = _maxHealth;
        }
        
        healthBar.Health = health;

      
    }

    void Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SortingGroup>().sortingOrder = -10;
        GetComponentInChildren<Gun>().enabled = false;
        IsAlive = false;
        // désactiver contrôles, animations, etc.
    }
    
    
}