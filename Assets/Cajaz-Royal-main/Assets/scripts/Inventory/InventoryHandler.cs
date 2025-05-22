using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class InventoryHandler : MonoBehaviourPunCallbacks
{
    private Inventory inventory;
    [SerializeField] private GameObject uiInventoryCanvas;
    [SerializeField] private UI_Inventory uiInventory;
    [SerializeField] private GameObject handGameObject;
    
    [SerializeField] private Sprite[] customInventory = new Sprite[16];

    private SpriteRenderer _handSpriteRenderer;
    private Gun _gun;

    private Item _equiped;
    
    private void Awake()
    {
        ItemLoader.Load("Assets/ItemReferences/items.json"); 
        _handSpriteRenderer = handGameObject.transform.Find("Visual").GetComponentInChildren<SpriteRenderer>();
        _gun = handGameObject.transform.GetComponent<Gun>();
        
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        Item[] items = new Item[16];
        
        for (int i = 0; i < 16; i++)
        {
            if (customInventory[i] != null)
            {
                var entrieParams =
                    (from entrie in ItemLoader.Entries
                        where entrie.ID.ToString() == customInventory[i].name.Split("_")[0]
                        select entrie).First();
                
                ItemType itemType;
                switch (entrieParams.Type)
                {
                    case "Arme": itemType = ItemType.Arme; break;
                    case "Casque": itemType = ItemType.Casque; break;
                    case "Armure": itemType = ItemType.Armure; break;
                    case "Bas": itemType = ItemType.Bas; break;
                    default: itemType = ItemType.Divers; break;
                }

                items[i] = new Item(
                    customInventory[i],
                    1,
                    entrieParams.Name,
                    itemType,
                    entrieParams.Parameters
                    ); 
            }
        }

        inventory = new Inventory(items);
    }
    
    private void Save([CanBeNull] Item item) 
    {
        if (_equiped != null)
        {
            _equiped.Save = new Dictionary<string, float>()
            {
                {"Munition", _gun.Munition}
            };
        }
    }

    private void Restore([CanBeNull] Item item)
    {
        if (item != null)
        {
            if (item.Save != null) _gun.Munition = (int)item.Save["Munition"];
            else _gun.Munition = _gun.CapaciteeChargeur;
        }
        
    }
    
    private void Equip([CanBeNull] Item item)
    {
        // Save the current _equiped to the inventory
        Save(_equiped);
        
        // Equip the new one
        if (item != null)
        {
            var parameters = item.attributes;

            foreach (var (attribute, value) in parameters)
            {
                switch (attribute)
                {
                    case "damage": _gun.damage = (int)value; break;
                    case "range": _gun.Range = (int)value; break;
                    case "fireRate": _gun.Firerate = value; break;
                    case "ammo" : _gun.CapaciteeChargeur = (int)value; break;
                }
            }
            Restore(item);
            _equiped = item;
            
            Debug.Log($"Equipped {item.name}");
        }
        else Debug.Log($"Equipped nothing");
        _handSpriteRenderer.sprite = item?.sprite;
    }

    private void UpdateHand()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(inventory[3]);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Equip(inventory[4]);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Equip(inventory[5]);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Equip(inventory[6]);
        if (Input.GetKeyDown(KeyCode.Alpha5)) Equip(inventory[7]);
    }
    
    void Update()
    {
        UpdateHand();
        
        if (Input.GetKeyDown(KeyCode.I)) 
            uiInventoryCanvas.gameObject.SetActive(!uiInventoryCanvas.gameObject.activeSelf);

        if (uiInventoryCanvas.gameObject.activeSelf)
        {
            UpdateInventory();
            
            uiInventory.SetInventory(inventory);
            uiInventory.RefreshInventory(); 
        }
    }
}