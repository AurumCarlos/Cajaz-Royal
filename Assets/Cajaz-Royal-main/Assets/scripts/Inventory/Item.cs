using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Casque,
    Armure,
    Bas,
    Arme,
    Divers,
} 

public class Item 
{
    public Sprite sprite;
    public int amount;

    public string name;
    public ItemType itemType;

    public Dictionary<string, float> attributes;

    public Dictionary<string, float> Save { get; set; }

    public Item(Sprite sprite, int amount, string name, ItemType itemType, Dictionary<string, float> attributes)
    {
        this.sprite = sprite;
        this.amount = amount;
        this.name = name;
        this.itemType = itemType;
        this.attributes = attributes;
    }
}
