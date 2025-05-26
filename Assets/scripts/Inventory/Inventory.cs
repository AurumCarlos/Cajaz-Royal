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

    public int? FindAvailablePlace(int startingPos, int finishPos = 16)
    {
        for (int i = startingPos; i < finishPos; ++i)
        {
            if (_items[i] == null) return i;
        }
        return null;
    }
    public bool AddItem(Item item)
    {
        if (item == null) return false;

        if (item.IsStackable())
        {
            foreach (var storedItem in _items)
            {
                if (storedItem == item)
                {
                    storedItem.Amount += item.Amount;
                    return true;
                }
            }
        }
        
        switch (item.ItemType)
        {
            case ItemType.Casque:
            {
                if (_items[0] != null)
                {
                    int? index = FindAvailablePlace(8);
                    if (index != null) _items[index.Value] = item;
                    else return false;
                }
                else _items[0] = item;
                return true;
            }
            case ItemType.Armure:
            {
                if (_items[1] != null)
                {
                    int? index = FindAvailablePlace(8);
                    if (index != null) _items[index.Value] = item;
                    else return false;
                }
                else _items[1] = item;
                return true;
            }
            case ItemType.Bas:
            {
                if (_items[2] != null)
                {
                    int? index = FindAvailablePlace(8);
                    if (index != null) _items[index.Value] = item;
                    else return false;
                }
                else _items[2] = item;
                return true;
            }
            case ItemType.Arme:
            {
                int? index = FindAvailablePlace(3);
                if (index != null)
                {
                    _items[index.Value] = item;
                    return true;
                } 
                return false;
            }
                
            default:
            {
                int? index = FindAvailablePlace(8);
                if (index != null)
                {
                    _items[index.Value] = item;
                    return true;
                } 
                return false;
            }
        }
        
    }
    
    public IEnumerator<Item> GetEnumerator()
    {
        foreach (var item in _items) yield return item;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
