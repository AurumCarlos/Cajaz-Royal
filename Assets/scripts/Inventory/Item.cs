using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ItemType
{
    Casque,
    Armure,
    Bas,
    Arme,
    Munitions,
    Soins,
    Plate
} 

public class Item 
{
    public int ID { get; }
    public Sprite Sprite { get; }
    public int Amount { get; set; }

    public string Name { get; }
    public ItemType ItemType { get; }

    public Dictionary<string, float> Attributes { get; }
    
    public Dictionary<string, float> Save { get; set; }

    public bool IsStackable()
    {
        switch (ItemType)
        {
            case ItemType.Casque: 
            case ItemType.Armure:
            case ItemType.Bas:
            case ItemType.Arme: 
                return false;
            default: 
                return true;
        }
    }
    
    public Item(int id, Sprite sprite, int amount, string name, ItemType itemType, Dictionary<string, float> attributes)
    {
        ID = id;
        Sprite = sprite;
        Amount = amount;
        Name = name;
        ItemType = itemType;
        Attributes = attributes;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is Item other)
            return Name == other.Name;

        return false;
    }

    public override int GetHashCode() => Name.GetHashCode();
    
    public static bool operator ==(Item i1, Item i2)
    {
        if ((object)i1 == null && (object)i2 == null) return true;
        if ((object)i1 == null || (object)i2 == null) return false;
        return i1.Name == i2.Name;
    }

    public static bool operator !=(Item i1, Item i2) => !(i1 == i2);

    public static Item Copy(Item item)
    {
        return new Item(item.ID, item.Sprite, item.Amount, item.Name, item.ItemType, item.Attributes);
    }
    
}
