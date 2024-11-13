using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersHandler : MonoBehaviour
{
    public GameObject[] equipedLayers;

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject layer in equipedLayers)
        {
            layer.SetActive(true);
        }
    }
}
