using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Photon.Pun;

public class LayersHandler : MonoBehaviourPunCallbacks
{
    private Dictionary<string, GameObject[]> _allLayers = new();
    [CanBeNull] public GameObject childToEquip;
    [CanBeNull] public GameObject parentToUnequip;

    void Start()
    {
        // Load all layers under child groups (helmets, armors, etc.)
        GameObject group, layer;
        for (int i = 0; i < gameObject.transform.childCount; i++) 
        {
            group = gameObject.transform.GetChild(i).gameObject;

            GameObject[] layers = new GameObject[group.transform.childCount];
            for (int j = 0; j < group.transform.childCount; j++) 
            {
                layer = group.transform.GetChild(j).gameObject;
                layer.SetActive(false); // Set all layers as inactive initially
                layers[j] = layer;
            }

            // Add the layers array to the dictionary with the group name as the key
            _allLayers.Add(group.name, layers);
        }
    }

    // Unequip all layers for a given parent (e.g., helmets or armor)
    void Unequip(string parentName)
    {
        if (_allLayers.ContainsKey(parentName)) 
        {
            foreach (GameObject layer in _allLayers[parentName])
            {
                layer.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning($"No layers found for {parentName}");
        }
        parentToUnequip = null; // Reset after unequipping
    }

    // Equip a specific item (layer) and deactivate others
    void Equip(GameObject equipment)
    {
        // Extract parent name from the equipment's name
        string[] inputs = equipment.name.Split('_');
        string parentName = inputs[0]; // The first part of the name represents the group

        // Unequip existing layers for this parent group
        Unequip(parentName);

        // Activate the selected equipment (layer)
        equipment.SetActive(true);
        Debug.Log($"Equipped {equipment.name}");
        
        childToEquip = null; // Reset after equipping
    }

    // Update is called once per frame
    void Update()
    {
        // Check if we need to equip a new layer
        if (childToEquip != null)
        {
            Equip(childToEquip);
        }

        // Check if we need to unequip a parent group
        if (parentToUnequip != null)
        {
            Unequip(parentToUnequip.name);
        }
    }
}