using UnityEngine;
using TMPro;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 50f;   
    [SerializeField] private float resetY = 1000f;      
    [SerializeField] private float startY = -500f;      

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
