using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarScroll : MonoBehaviour
{
    [SerializeField] private RawImage starImage;
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0, 0.02f); 

    private Vector2 uvOffset = Vector2.zero;

    void Update()
    {
        uvOffset += scrollSpeed * Time.deltaTime;
        starImage.uvRect = new Rect(uvOffset, starImage.uvRect.size);
    }
}