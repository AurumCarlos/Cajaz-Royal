using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class InventoryHandlerSolo : MonoBehaviour
{
    public Inventory Inventory { get; set; }
    [SerializeField] private GameObject uiInventoryCanvas;
    [SerializeField] public UI_InventorySolo uiInventory;
    [SerializeField] private UI_Shield uiShield;
    [SerializeField] private GameObject handGameObject;
    private PhotonView _handPhotonView;
    
    [SerializeField] private Sprite[] toSpawn;
    [SerializeField] private Sprite[] customInventory = new Sprite[16];

    private SpriteRenderer _handSpriteRenderer;
    private Gun _gun;

    public Item Equiped { get; set; }
    
    private void Start()
    {
        _handSpriteRenderer = handGameObject.transform.Find("Visual").GetComponentInChildren<SpriteRenderer>();
        _gun = handGameObject.transform.GetComponent<Gun>();
        _handPhotonView = handGameObject.transform.Find("Visual").GetComponentInChildren<PhotonView>();
        
        InitInventory();
        uiInventory.Inventory = Inventory;
        uiInventory.RefreshInventory();
    }
    
    private void InitInventory()
    {
        Item[] items = new Item[16];

        for (int i = 0; i < 16; i++)
        {
            if (customInventory[i] != null)
            {
                items[i] = ItemLoader.GetItemFromSprite(customInventory[i]);
            }
        }
        Inventory = new Inventory(items);
    }
    
    public void UpdateInventory()
    {
        Item[] items = new Item[16];
        
        for (int i = 0; i < 16; i++)
        {
            
            if (Inventory?[i] != null)
            {
                items[i] = Inventory[i];
            }
        }
        Inventory = new Inventory(items);
    }
    
    public void Save([CanBeNull] Item item) 
    {
        if (item != null)
        {
            item.Save = new Dictionary<string, float>()
            {
                {"Munition", _gun.Munition.Amount}
            };
        }
    }
    
    
    private void EquipWeapon([CanBeNull] Item item)
    {
        // Save the current _equiped to the inventory
        Save(Equiped);
        
        // Equip the new one
        if (item != null)
        {
            var parameters = item.Attributes;

            foreach (var (attribute, value) in parameters)
            {
                Debug.Log("Parsing " + (attribute, value));
                switch (attribute)
                {
                    case "damage": _gun.damage = (int)value; break;
                    case "range": _gun.Range = (int)value; break;
                    case "fireRate": _gun.Firerate = value; break;
                    case "ammo" : _gun.CapaciteeChargeur = (int)value; break;
                    case "ammoType": 
                        _gun.Munition = ItemLoader.GetItem((int)value, item.Save == null ? 
                        Random.Range(_gun.CapaciteeChargeur/2, _gun.CapaciteeChargeur+1) : 
                        (int)item.Save["Munition"]); 
                        break;
                }
            }
            Equiped = item;
        }
        else Debug.Log($"Equipped nothing");
        _handSpriteRenderer.sprite = item?.Sprite;
    }

    private void UpdateHand()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(Inventory[3]);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(Inventory[4]);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(Inventory[5]);
        if (Input.GetKeyDown(KeyCode.Alpha4)) EquipWeapon(Inventory[6]);
        if (Input.GetKeyDown(KeyCode.Alpha5)) EquipWeapon(Inventory[7]);
    }
    
    void Update()
    {
        UpdateHand();
        
        if (Input.GetKeyDown(KeyCode.I)) 
            uiInventoryCanvas.gameObject.SetActive(!uiInventoryCanvas.gameObject.activeSelf);

        uiInventory.Inventory = Inventory;
        if (uiInventoryCanvas.gameObject.activeSelf)
        {
            UpdateInventory();
            uiInventory.Inventory = Inventory;
            uiInventory.RefreshInventory(); 
        }
        
        uiInventory.RefreshEquipments();
        uiShield.Shield = Inventory[1];
    }
}