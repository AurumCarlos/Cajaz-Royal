using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class InventoryHandler : MonoBehaviourPunCallbacks
{
    public Inventory Inventory { get; set; }
    [SerializeField] private GameObject uiInventoryCanvas;
    [SerializeField] public UI_Inventory uiInventory;
    [SerializeField] private UI_Shield uiShield;
    [SerializeField] private GameObject handGameObject;
    private PhotonView _handPhotonView;
    
    [SerializeField] private Sprite[] toSpawn;
    [SerializeField] private Sprite[] customInventory = new Sprite[16];

    private SpriteRenderer _handSpriteRenderer;
    private Gun _gun;

    public Item Equiped { get; set; }
    
    private void Start()
    {
        _handSpriteRenderer = handGameObject.transform.Find("Visual").GetComponentInChildren<SpriteRenderer>();
        _gun = handGameObject.transform.GetComponent<Gun>();
        _handPhotonView = handGameObject.transform.Find("Visual").GetComponentInChildren<PhotonView>();
        
        InitInventory();
        uiInventory.Inventory = Inventory;
        uiInventory.RefreshInventory();
    }
    
    private void InitInventory()
    {
        Item[] items = new Item[16];

        for (int i = 0; i < 16; i++)
        {
            if (customInventory[i] != null)
            {
                items[i] = ItemLoader.GetItemFromSprite(customInventory[i]);
            }
        }
        Inventory = new Inventory(items);
    }
    
    public void UpdateInventory()
    {
        Item[] items = new Item[16];
        
        for (int i = 0; i < 16; i++)
        {
            
            if (Inventory?[i] != null)
            {
                items[i] = Inventory[i];
            }
        }
        Inventory = new Inventory(items);
    }

    private void SaveEquipement([CanBeNull] Item item)
    {
        if (item != null)
        {
            item.Save = new Dictionary<string, float>()
            {
                { "Plates", item.Attributes["Plates"] }
            };
        }
    }
    public void Save([CanBeNull] Item item) 
    {
        if (item != null)
        {
            item.Save = new Dictionary<string, float>()
            {
                {"Munition", _gun.Munition.Amount}
            };
        }
    }
    
    
    private void EquipWeapon([CanBeNull] Item item)
    {
        // Save the current _equiped to the inventory
        Save(Equiped);
        
        // Equip the new one
        if (item != null)
        {
            var parameters = item.Attributes;

            foreach (var (attribute, value) in parameters)
            {
                Debug.Log("Parsing " + (attribute, value));
                switch (attribute)
                {
                    case "damage": _gun.damage = (int)value; break;
                    case "range": _gun.Range = (int)value; break;
                    case "fireRate": _gun.Firerate = value; break;
                    case "ammo" : _gun.CapaciteeChargeur = (int)value; break;
                    case "ammoType": 
                        _gun.Munition = ItemLoader.GetItem((int)value, item.Save == null ? 
                        Random.Range(_gun.CapaciteeChargeur/2, _gun.CapaciteeChargeur+1) : 
                        (int)item.Save["Munition"]); 
                        break;
                }
            }
            Equiped = item;
        }
        else Debug.Log($"Equipped nothing");
        photonView.RPC("ChangeWeaponRPC", RpcTarget.All, transform.GetComponentInParent<PhotonView>().ViewID, item?.ID);
    }

    private void UpdateHand()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(Inventory[3]);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(Inventory[4]);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(Inventory[5]);
        if (Input.GetKeyDown(KeyCode.Alpha4)) EquipWeapon(Inventory[6]);
        if (Input.GetKeyDown(KeyCode.Alpha5)) EquipWeapon(Inventory[7]);
    }

    [PunRPC] 
    public void ChangeWeaponRPC(int playerViewId, int itemId) 
    { 
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) {
            if (player.GetComponent<PhotonView>().ViewID == playerViewId) {
                Item item = ItemLoader.GetItem(itemId);
                player.transform.Find("Aim").Find("Visual").GetComponentInChildren<SpriteRenderer>().sprite = item?.Sprite;
                break;
            }
        }
    }

    [PunRPC]
    public void ChangeCasqueRPC(int playerViewId, int itemId) { 
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) {
            if (player.GetComponent<PhotonView>().ViewID == playerViewId) {
                if (itemId == -1) { 
                    foreach (Transform child in player.transform.Find("casque")) child.gameObject.SetActive(false);
                }
                Item item = ItemLoader.GetItem(itemId);
                player.transform.Find("Aim").Find("Visual").GetComponentInChildren<SpriteRenderer>().sprite = item?.Sprite;
                break;
            }
        }
    }
    
    [PunRPC]
    public void Unequip(int playerViewId, string equipement)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player.GetComponent<PhotonView>().ViewID == playerViewId)
            {
                var group = player.transform.Find(equipement);
                foreach (Transform child in group) child.gameObject.SetActive(false);
                
                break;
            }
        }
    }

    [PunRPC]
    public void Equip(int playerViewId, int groupIndex, int spriteIndex)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var photonView = player.GetComponent<PhotonView>();
            if (photonView == null) { Debug.LogError("[Equip] PhotonView null"); continue; }
            if (photonView.ViewID != playerViewId) continue;

            string equipement;
            switch (groupIndex)
            {
                case 0: equipement = "casque"; break;
                case 1: equipement = "armor"; break;
                case 2: equipement = "bas"; break;
                default: equipement = "casque"; break;
            }

            var group = player.transform.Find(equipement);
            if (group == null) { Debug.LogError("[Equip] Group null"); break; }

            if (spriteIndex == -1)
            {
                foreach (Transform child in group) child.gameObject.SetActive(false);
            }
            else
            {
                foreach (Transform child in group)
                {
                    if (child.gameObject.name == spriteIndex.ToString()) child.gameObject.SetActive(true);
                }
            }
            
            break;
        }
    }

    [CanBeNull]
    private Sprite GetCurrent(string equipement)
    {
        var group = transform.Find(equipement);
        foreach (Transform child in group) if (child.gameObject.activeSelf) return child.GetComponent<SpriteRenderer>().sprite;
        return null;
    }

    public void RefreshCasque()
    {
        var currentCasque = GetCurrent("casque");
        if (Inventory[0] == null)
        {
            if (currentCasque != null) 
                photonView.RPC("Unequip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, "casque");  //Unequip("casque");
        }
        else if (ItemLoader.GetIdFromSprite(Inventory[0].Sprite) != ItemLoader.GetIdFromSprite(currentCasque))
        {
            photonView.RPC("Unequip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, "casque");
            photonView.RPC("Equip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, 0, ItemLoader.GetIdFromSprite(Inventory[0].Sprite));
        }
    }
    
    public void RefreshArmure()
    {
        var currentCasque = GetCurrent("armor");
        //uiShield.Shield = Inventory[1];
        if (Inventory[1] == null)
        {
            if (currentCasque != null) 
                photonView.RPC("Unequip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, "armor");  //Unequip("casque");
        }
        else if (ItemLoader.GetIdFromSprite(Inventory[1].Sprite) != ItemLoader.GetIdFromSprite(currentCasque))
        {
            photonView.RPC("Unequip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, "armor");
            
            photonView.RPC("Equip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, 1,
                ItemLoader.GetIdFromSprite(Inventory[1].Sprite));
        }
    }
    
    public void RefreshBas()
    {
        var currentCasque = GetCurrent("bas");
        if (Inventory[2] == null)
        {
            if (currentCasque != null) 
                photonView.RPC("Unequip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, "bas");  //Unequip("casque");
        }
        else if (ItemLoader.GetIdFromSprite(Inventory[2].Sprite) != ItemLoader.GetIdFromSprite(currentCasque))
        {
            photonView.RPC("Unequip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, "bas");
            photonView.RPC("Equip", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, 2, ItemLoader.GetIdFromSprite(Inventory[2].Sprite));
        }
    }

    public void RefreshEquipements()
    {
        RefreshCasque();
        RefreshArmure();
        RefreshBas();
    }
    
    void Update()
    {
        UpdateHand();
        
        if (Input.GetKeyDown(KeyCode.I)) 
            uiInventoryCanvas.gameObject.SetActive(!uiInventoryCanvas.gameObject.activeSelf);

        uiInventory.Inventory = Inventory;
        if (uiInventoryCanvas.gameObject.activeSelf)
        {
            UpdateInventory();
            uiInventory.Inventory = Inventory;
            uiInventory.RefreshInventory(); 
        }
        
        //uiInventory.RefreshEquipments();
        
        RefreshEquipements();
        uiShield.Shield = Inventory[1];
    }
}