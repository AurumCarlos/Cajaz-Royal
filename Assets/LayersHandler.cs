using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersHandler : MonoBehaviour
{
    public GameObject[] equipedLayers = new GameObject[4];
    public GameObject toEquip;

    private static int GetIndexObject(GameObject child)
    {
        GameObject parent = child.transform.parent.gameObject;
        for (int i = 0; i < parent.transform.childCount; i++) 
        {
            if (GameObject.ReferenceEquals(parent.transform.GetChild(i), child)) return i;
        }

        return -1;
    }
    
    void equip(GameObject equipement)
    {
        GameObject parent = equipement.transform.parent.gameObject;
        
        for (int i = 0; i < parent.transform.childCount; i++) 
        {
            parent.transform.GetChild(i).gameObject.SetActive(false);
        }
        equipedLayers[GetIndexObject(parent)] = equipement;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (toEquip != null) 
        {
            equip(toEquip);
            toEquip = null;
        }
        
        foreach (GameObject layer in equipedLayers)
        {
            layer.SetActive(true);
        }
    }
}
