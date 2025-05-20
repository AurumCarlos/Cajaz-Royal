using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform _slots;
    private List<Transform> _quickSlots;
    private List<Transform> _inventorySlots;
    private (Transform casque, Transform armure, Transform bas) _equipmentSlots;
    
    private void Awake()
    {
        _slots = transform.Find("slots");
        
        _quickSlots = new List<Transform>();
        _inventorySlots = new List<Transform>();
        
        foreach (Transform slot in _slots)
        {
            if (slot.name.Contains("main")) _quickSlots.Add(slot);
            else if (slot.name.Contains("slot")) _inventorySlots.Add(slot);
        }

        _equipmentSlots = (_slots.Find("casque"), _slots.Find("armure"), _slots.Find("bas"));
    }
    
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }
    
    public void RefreshInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null) _slots.GetChild(i).GetComponentInChildren<Image>().sprite = inventory[i].sprite;
        }
    }
}
