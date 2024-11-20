using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersHandler : MonoBehaviour
{
    private Dictionary<string,GameObject[]> allLayers = new Dictionary<string, GameObject[]>();
    public GameObject toEquip;
    public GameObject toUnequip;
    
    void Start()
    {
        //string debug = "";
        
        // loads all layers 
        GameObject group, layer;
        for (int i = 0; i < gameObject.transform.childCount; i++) 
        {
            group = gameObject.transform.GetChild(i).gameObject;
            //debug += group.name + "\n";
            
            GameObject[] layers = new GameObject[group.transform.childCount];
            for (int j = 0; j < group.transform.childCount; j++) 
            {
                // debug += " | " + group.transform.GetChild(j).gameObject.name + "\n";
                layer = group.transform.GetChild(j).gameObject;
                layer.SetActive(false);
                layers[j] = layer; 
            }
            
            allLayers.Add(group.name, layers);
        }
        //print(debug);
        
    }

    void UnequipGroup(GameObject group)
    {
        Unequip(group.name);
    }
    
    void Unequip(string groupName)
    {
        foreach (GameObject layer in allLayers[groupName]) layer.SetActive(false);
    }
    
    void Equip(GameObject equipement)
    {
        // Equipements data
        string[] inputs = equipement.name.Split('_');

        Unequip(inputs[0]);
        equipement.SetActive(true);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (toEquip != null)
        {
            Equip(toEquip);
            toEquip = null;
        }

        if (toUnequip != null)
        {
            UnequipGroup(toUnequip);
            toUnequip = null;
        }
    }
}
