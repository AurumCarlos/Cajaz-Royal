using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UI_InventorySolo : MonoBehaviour
{
    public Inventory Inventory { get; set; }
    private Transform _slots;

    [SerializeField] private GameObject PlayerSprite;
    private HealthSolo _health;
    
    private void Start()
    {
        _health = PlayerSprite.GetComponent<HealthSolo>();
        _slots = transform.Find("slots");
    }

    public void Unequip(string equipement)
    {
        var group = PlayerSprite.transform.Find(equipement);
        foreach (Transform child in group) child.gameObject.SetActive(false);
    }

    [CanBeNull]
    private Sprite GetCurrent(string equipement)
    {
        var group = PlayerSprite.transform.Find(equipement);
        foreach (Transform child in group) if (child.gameObject.activeSelf) return child.GetComponent<SpriteRenderer>().sprite;
        return null;
    }

    public void RefreshCasque()
    {
        var currentCasque = GetCurrent("casque");
        if (Inventory[0] == null)
        {
            if (currentCasque != null) Unequip("casque");
        }
        else if (ItemLoader.GetIdFromSprite(Inventory[0].Sprite) != ItemLoader.GetIdFromSprite(currentCasque))
        {
            Unequip("casque");
            var group = PlayerSprite.transform.Find("casque");
            foreach (Transform child in group) 
                if (Inventory[0].Sprite == child.GetComponent<SpriteRenderer>().sprite) 
                    child.gameObject.SetActive(true);
        }
    }
    
    public void RefreshArmure()
    {
        var currentArmure = GetCurrent("armor");;
        if (Inventory[1] == null)
        {
            if (currentArmure != null) Unequip("armor");
        }
        else if (ItemLoader.GetIdFromSprite(Inventory[1].Sprite) != ItemLoader.GetIdFromSprite(currentArmure)) 
        {
            Unequip("armor");
            var group = PlayerSprite.transform.Find("armor");
            foreach (Transform child in group) 
                if (Inventory[1].Sprite == child.GetComponent<SpriteRenderer>().sprite) 
                    child.gameObject.SetActive(true);
        }
    }
    
    public void RefreshBas()
    {
        var currentBas = GetCurrent("bas");;
        if (Inventory[2] == null)
        {
            if (currentBas != null) Unequip("bas");
        }
        else if (ItemLoader.GetIdFromSprite(Inventory[2].Sprite) != ItemLoader.GetIdFromSprite(currentBas))
        {
            Unequip("bas");
            var group = PlayerSprite.transform.Find("bas");
            foreach (Transform child in group) 
                if (Inventory[2].Sprite == child.GetComponent<SpriteRenderer>().sprite) 
                    child.gameObject.SetActive(true);
        }
    }
    
    public void RefreshEquipments()
    {
        RefreshCasque();
        RefreshArmure();
        RefreshBas();
    }
    
    public void RefreshInventory()
    {
        if (_slots == null) Start();
        
        for (int i = 0; i < Inventory.Length; i++)
        {
            var currSlot = _slots.transform.GetChild(i);
            if (Inventory[i] == null)
            {
                currSlot.Find("SpriteDisplay").GetComponent<Image>().sprite = null;
                currSlot.Find("NameSlot").GetComponent<TextMeshProUGUI>().text = "";

                if (currSlot.Find("AmountSlot") != null)
                {
                    currSlot.Find("AmountSlot").GetComponent<TextMeshProUGUI>().text = "";
                }
            }
            else
            {
                if (Inventory[i].Amount <= 0)
                {
                    currSlot.Find("SpriteDisplay").GetComponent<Image>().sprite = null;
                    currSlot.Find("NameSlot").GetComponent<TextMeshProUGUI>().text = "";

                    if (currSlot.Find("AmountSlot") != null)
                    {
                        currSlot.Find("AmountSlot").GetComponent<TextMeshProUGUI>().text = "";
                    }
                }
                else
                {
                    _slots.transform.GetChild(i).GetComponentInChildren<Image>().sprite = Inventory[i].Sprite;
                    currSlot.Find("NameSlot").GetComponent<TextMeshProUGUI>().text = Inventory[i].Name;

                    if (Inventory[i].Amount > 1)
                    {
                        currSlot.Find("AmountSlot").GetComponent<TextMeshProUGUI>().text = $"x{Inventory[i].Amount}";
                    }
                }
            }
        }
    }

    public void SpawnWorldItem(Vector3 position, int itemId, int amount)
    {
        Item item = ItemLoader.GetItem(itemId, amount);
        if (item != null)
        {
            var go = Instantiate(ItemHandler.Instance.toSpawn, position, Quaternion.identity);
            WorldItemSolo worldItem = go.GetComponent<WorldItemSolo>();
            worldItem.Item = item;
        }
    }
    
    private void Drop(Vector3 position, Item item)
    {
        Vector3 dir = Utils.GetRandomDir();
        //WorldItem.SpawnWorldItem(position + dir * 1.5f, item.ID, item.Amount);
        SpawnWorldItem(position + dir, item.ID, item.Amount);
    }

    private bool Use(int index)
    {
        var item = Inventory[index];
        switch (item.ItemType)
        {
            case ItemType.Soins:
            {
                _health.Damage(-(int)item.Attributes["heal"]);
                
                item.Amount--;
                return true;
            }
            case ItemType.Plate:
            {
                if (Inventory[1] != null)
                {
                    if (Inventory[1].Save == null)
                    {
                        if (Inventory[1].Attributes["plateCapacity"] > 0)
                        {
                            Inventory[1].Save = new()
                            {
                                { "Plates", 1 }
                            };
                            item.Amount--;
                        }
                        else return false;
                    }
                    else
                    {
                        if (Inventory[1].Save["Plates"] < Inventory[1].Attributes["plateCapacity"])
                        {
                            Inventory[1].Save["Plates"]++;
                            item.Amount--;
                        } 
                        else return false;
                    }
                    return true;
                }
                return false;
            }
            case ItemType.Casque:
            {
                if (index == 0) return false;
                Swap(index, 0);
                return true;
            }
            case ItemType.Armure:
            {
                if (index == 1) return false;
                Swap(index, 1);
                return true;
            }
            case ItemType.Bas:
            {
                if (index == 2) return false;
                Swap(index, 2);
                return true;
            }
            case ItemType.Arme:
            {
                int? newIndex;
                if (index < 8) // On veut desequiper
                {
                    newIndex = Inventory.FindAvailablePlace(8);
                }
                else // On veut equiper
                {
                    newIndex = Inventory.FindAvailablePlace(3, 8);
                }
                if (newIndex == null) return false;
                Swap(index, newIndex.Value);
                return true;
            }
        }

        return false;
    }
    
    private void Swap(int index1, int index2) => (Inventory[index1], Inventory[index2]) = (Inventory[index2], Inventory[index1]);

    private void Update()
    {
        for (int i = 0; i < _slots.transform.childCount; i++)
        {
            Transform slot = _slots.transform.GetChild(i);
            var slotItem = Inventory[i];
            int j = i;
            
            slot.GetComponent<Button_UI>().ClickFunc = () =>
            {
                try
                {
                    // Use Item or Equip Equipement
                    if (slotItem != null)
                    {
                        if (Use(j)) RefreshInventory();
                    }
                } catch (NullReferenceException) { }
                
            };
            slot.GetComponent<Button_UI>().MouseRightClickFunc = () =>
            {
                try
                {
                    // Drop Item
                    if (slotItem != null)
                    {
                        Drop(PlayerSprite.transform.position, slotItem);
                        Inventory[j] = null;
                        
                        Item? Equiped;
                        if ((Equiped = PlayerSprite.GetComponent<InventoryHandler>().Equiped) == slotItem)
                        {
                            PlayerSprite.GetComponent<InventoryHandler>().Save(Equiped);
                            PlayerSprite.GetComponent<InventoryHandler>().Equiped = null;
                        }
                        RefreshInventory();
                    }
                }
                catch (NullReferenceException) { }
            };
            slot.GetComponent<Button_UI>().MouseOverFunc = () =>
            {
                try
                {
                    // Equip Weapon
                    if (slotItem != null && slotItem.ItemType == ItemType.Arme)
                    {
                        if (Input.GetKeyDown(KeyCode.Alpha1)) Swap(j, 3);
                        if (Input.GetKeyDown(KeyCode.Alpha2)) Swap(j, 4);
                        if (Input.GetKeyDown(KeyCode.Alpha3)) Swap(j, 5);
                        if (Input.GetKeyDown(KeyCode.Alpha4)) Swap(j, 6);
                        if (Input.GetKeyDown(KeyCode.Alpha5)) Swap(j, 7);
                    }
                } catch (NullReferenceException) { }
            };
        }
    }
}
