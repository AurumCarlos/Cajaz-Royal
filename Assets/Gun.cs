using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    // Start is called before the first frame update
    
    private Vector2 mousePos;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        

        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x)*Mathf.Rad2Deg -0f;

        
        bool is_seeing_right = mousePos[0] >= gameObject.transform.position[0];
        // Rotation while looking
        //if (is_seeing_right) gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        //else gameObject.transform.rotation = Quaternion.Euler(0, 180f, angle-180f);
        
        
        transform.localRotation = Quaternion.Euler((!is_seeing_right ? 180f : 0f),(!is_seeing_right ? 180f : 0f),(!is_seeing_right ? -angle: angle));
    }
}
