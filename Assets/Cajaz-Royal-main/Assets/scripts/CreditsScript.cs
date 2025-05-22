using UnityEngine;
using TMPro;

public class CreditsScript : MonoBehaviour
{
    public float scrollSpeed = 50f;   
    public float resetY = 1000f;      
    public float startY = -500f;      

    private RectTransform rectTransform;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = startY;
        rectTransform.anchoredPosition = pos;
    }

    void Update()
    {
        if (rectTransform == null) return;

        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        if (rectTransform.anchoredPosition.y >= resetY)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y = startY;
            rectTransform.anchoredPosition = pos;
        }
    }
}
