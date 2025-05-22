using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Inventory : IEnumerable<Item> {
    private Item[] _items;
    
    public int Length { get => _items.Length; } 
    public Item this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public Inventory()
    {
        _items = new Item[16];
    }
    
    public Inventory(Item[] items)
    {
        _items = items;
    }

    public int? FindAvailablePlace()
    {
        for (int i = 3; i < _items.Length; ++i)
        {
            if (_items[i] == null) return i;
        }
        return null;
    }
    public void AddItem(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Casque: _items[0] = item; break;
            case ItemType.Armure: _items[1] = item; break;
            case ItemType.Bas: _items[2] = item; break;

            default:
            {
                int? index = FindAvailablePlace();
                if (index != null) _items[index.Value] = item; 
                break;
            }
        }
    }
    
    public IEnumerator<Item> GetEnumerator()
    {
        foreach (var item in _items) yield return item;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
