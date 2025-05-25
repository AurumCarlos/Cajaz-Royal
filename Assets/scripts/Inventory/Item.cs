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
    public Sprite sprite{ get; set; }
    public int amount{ get; set; }

    public string name{ get; set; }
    public ItemType itemType{ get; set; }

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
