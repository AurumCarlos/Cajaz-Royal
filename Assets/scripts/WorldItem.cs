using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WorldItem : MonoBehaviourPunCallbacks
{
    private Item _item;
    private SpriteRenderer _spriteRenderer;
    private TextMeshPro _amount;
    
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            _spriteRenderer.sprite = _item.Sprite;
            _amount.text = _item.Amount > 1 ? $"x{_item.Amount}" : "";
            
            float scaleMult;
            switch (_item.ItemType)
            {
                case ItemType.Armure: case ItemType.Bas: case ItemType.Casque: scaleMult = 0.5f; break;
                case ItemType.Soins: case ItemType.Munitions: scaleMult = 1.5f; break;
                default: scaleMult = 1f; break;
            }
            transform.localScale = Vector3.one * (2 * scaleMult);
        }
    }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _amount = GetComponentInChildren<TextMeshPro>();
    }

    void Start()
    {
        // Initialisation pour tous les clients
        object[] data = photonView.InstantiationData;
        if (data != null && data.Length == 2)
        {
            int itemId = (int)data[0];
            int amount = (int)data[1];
            this.Item = ItemLoader.GetItem(itemId, amount);
        }
    }
}
