using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


public class ItemLoader : MonoBehaviour
{
    public static ItemLoader Instance { get; private set; }
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private GameObject itemsPrefab;
    private List<Sprite> _sprites;
    public static Dictionary<int, Item> Items;
    
    public class JsonEntry
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, float> Parameters { get;  set; }

        public override string ToString()
        {
            return $"[{ID}] name = {Name}, type = {Type} ({Parameters.Count} attributes)";
        }
    }
    private List<JsonEntry> _jsonEntries;
    public List<JsonEntry> Entries { get => _jsonEntries; }
    
    private void LoadSprites(GameObject prefab)
    {
        foreach (Transform group in prefab.transform)
        {
            foreach (Transform child in group)
            {
                _sprites.Add(child.GetComponent<SpriteRenderer>().sprite);
            }
        }
    }

    private void InstantiateItems()
    {
        Items = new ();
        foreach (Sprite sprite in _sprites)
        {
            int id = int.Parse(sprite.name.Split("_")[0]);
            var entrieParams =
                (from entrie in Instance.Entries
                    where entrie.ID == id
                    select entrie).First();
                
            ItemType itemType;
            switch (entrieParams.Type)
            {
                case "Arme": itemType = ItemType.Arme; break;
                case "Casque": itemType = ItemType.Casque; break;
                case "Armure": itemType = ItemType.Armure; break;
                case "Bas": itemType = ItemType.Bas; break;
                case "Munitions":
                {
                    itemType = ItemType.Munitions; 
                    Debug.Log($"[LOADED MUNITIONS] {id}");
                    break;
                }
                case "Soins": itemType = ItemType.Soins; break;
                case "Plate": itemType = ItemType.Plate; break;
                default: 
                    itemType = ItemType.Arme; break;
            }

            Items.Add(id, new Item(
                id,
                sprite,
                1,
                entrieParams.Name,
                itemType,
                entrieParams.Parameters
            ));
            
        }
    }

    void Awake()
    {
        Instance = this;
        // load json file
        string path = Path.Combine(Application.streamingAssetsPath, "items.json");
        Load(path);
        //Load("Assets/ItemReferences/items.json");
        
        
        // load sprites
        _sprites = new List<Sprite>();
        LoadSprites(weaponPrefab);
        LoadSprites(playerPrefab);
        LoadSprites(itemsPrefab);
        
        // link sprites and entries
        InstantiateItems();
    }
    public void Load(string path)
    {
        string json = File.ReadAllText(path);
        _jsonEntries = JsonConvert.DeserializeObject<List<JsonEntry>>(json);
    }
    
    public static int GetIdFromSprite([CanBeNull] Sprite sprite) => sprite == null ? -1 : int.Parse(sprite.name.Split("_")[0]);

    [CanBeNull]
    public static Item GetItem(int id,  int? amount = null)
    {
        var result = Items.ContainsKey(id) ? Items[id] : null;

        if (result != null)
        {
            var output = Item.Copy(result);
            output.Amount = amount ?? 1;
            return output;
        }
        return null;
        
    }
    
    public static Item GetItemFromSprite(Sprite sprite, int? amount = null)
    {
        int id = GetIdFromSprite(sprite);
        return GetItem(id, amount);
    }
}
