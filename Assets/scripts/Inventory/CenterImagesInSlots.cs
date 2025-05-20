using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CenterImagesInSlots : MonoBehaviour
{
    void Update()
    {
        foreach (Transform child in transform)
        {
            Image im = child.GetComponentInChildren<Image>();
            if (im != null)
            {
                im.color = im.sprite == null ? new Color(255, 255, 255, 0): new Color(255, 255, 255, 255);
                im.preserveAspect = true;
            }
        }
    }
}