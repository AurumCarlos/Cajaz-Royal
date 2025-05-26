using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnItemsSolo : MonoBehaviour
{
    [SerializeField] public bool open = false;
    [SerializeField] public GameObject toSpawn;
    [SerializeField] public Sprite[] customChest;
    
    [SerializeField] private Texture2D[] onHoverCursor;
    
    [SerializeField] private float holdTime = 2f;        
    private float _holdTimer = 0f;
    private bool _isHovering = false;

    private int _currentFrame;
    private float _frameTimer;
    private float _frameRate;
    private Item[] _items;

    private WorldItem _tmp;
    private bool playerIsInside = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = false;
            OnMouseExit();
        }
    }

    private void GenerateLoot()
    {
        int count = Random.Range(3, 6);
        Item? curr;
        
        List<Item> toCreate = new List<Item>();
        if (Random.value < 0.4f && (curr = ItemLoader.GetItem(Random.Range(1, 51))) != null) toCreate.Add(curr);  // Weapon
        else if ((curr = ItemLoader.GetItem(Random.Range(85, 88), Random.Range(6, 14))) != null) toCreate.Add(curr); // Munitions
        
        if ((curr = ItemLoader.GetItem(Random.Range(85, 88), Random.Range(11, 44))) != null) toCreate.Add(curr); // Munitions
        if (Random.value < 0.5f) toCreate.Add(ItemLoader.GetItem(Random.Range(88, 91))); // Medkit
        else toCreate.Add(ItemLoader.GetItem(91, Random.Range(1, 2))); // Plate
        
        
        if (count > 3 && (curr = ItemLoader.GetItem(Random.Range(1, 51))) != null) toCreate.Add(curr);  // Weapon
        if (count > 4 && (curr = ItemLoader.GetItem(Random.Range(51, 85))) != null) toCreate.Add(curr); // Equipement
        
        Utils.ShuffleList(toCreate, 8);
        _items = new Item[toCreate.Count];
        for (int i = 0; i < toCreate.Count; i++)
        {
            _items[i] = toCreate[i];
        }
    }
    
    void Start()
    {
        _frameRate = holdTime / onHoverCursor.Length;
        if (customChest.Length == 0) GenerateLoot();
        else
        {
            int count = customChest.Length;
            _items = new Item[count];
            for (int i = 0; i < count; i++)
            {
                _items[i] = ItemLoader.GetItemFromSprite(customChest[i]);
            }
        }
    }
    
    void OnMouseOver()
    {
        _isHovering = true;
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, new Vector3(1, 1), CursorMode.Auto);
        _isHovering = false;
        _holdTimer = 0f; // Reset du timer si on sort du survol
        _currentFrame = 0;
        _frameTimer = 0f;
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

    private void Open()
    {
        var pos = transform.position;
        Cursor.SetCursor(null, new Vector3(1, 1), CursorMode.Auto);
        Destroy(gameObject);
        
        foreach (var item in _items)
        {
            Vector3 dir = Utils.GetRandomDir();
            SpawnWorldItem(pos + dir, item.ID, item.Amount);
        }
    }

    void Update()
    {
        if (open) Open();
        if (!playerIsInside) return;
        if (_isHovering)
        {
            if (Input.GetKey(KeyCode.E))
            {
                _holdTimer += Time.deltaTime;
                _frameTimer -= Time.deltaTime;

                if (_holdTimer >= holdTime)
                {
                    Open();
                    return;
                }

                if (_frameTimer <= 0)
                {
                    _frameTimer += _frameRate;
                    _currentFrame = (_currentFrame + 1) % onHoverCursor.Length;
                    Cursor.SetCursor(onHoverCursor[_currentFrame], new Vector2(1, 1), CursorMode.Auto);
                }
            }
            else
            {
                _currentFrame = 0;
                _frameTimer = 0;
                Cursor.SetCursor(onHoverCursor[_currentFrame], new Vector2(1, 1), CursorMode.Auto);
                _holdTimer = 0f; // Reset si la touche est relâchée
            }
        }
    }
}
