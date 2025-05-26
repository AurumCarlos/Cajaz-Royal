#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UI_Shield : MonoBehaviour
{
    private List<GameObject> _backgrounds;
    private List<GameObject> _plates;
    
    public Item? Shield
    {
        set
        {
            Item? item = value;
            if (item == null)
            {
                ResetHUD();
                return;
            }
                
            for (int i = 0; i < item.Attributes["plateCapacity"]; i++) _backgrounds[i].SetActive(true);
            for (int i = (int)item!.Attributes["plateCapacity"]; i < _backgrounds.Count; i++) _backgrounds[i].SetActive(false);
            
            if (item.Save == null) for (int i = 0; i < _plates.Count; i++) _plates[i].SetActive(false);
            else
            {
                for (int i = 0; i < item.Save["Plates"]; i++) _plates[i].SetActive(true);
                for (int i = (int)item!.Save["Plates"]; i < _plates.Count; i++) _plates[i].SetActive(false);
            }
        }
    }

    private void ResetHUD()
    {
        foreach (var obj in _backgrounds) obj.SetActive(false);
        foreach (var obj in _plates) obj.SetActive(false);
    }

    void Awake()
    {
        _backgrounds = new List<GameObject>();
        _plates = new List<GameObject>();
        
        for (int i = 0; i < 3; i++) _backgrounds.Add(transform.GetChild(i).gameObject);
        for (int i = 3; i < transform.childCount; i++) _plates.Add(transform.GetChild(i).gameObject);
    }
}
