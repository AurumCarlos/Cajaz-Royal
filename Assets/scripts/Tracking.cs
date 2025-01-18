using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracking : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void cameraFollow()
    {
        float charPosX = transform.position.x;
        float charPosY = transform.position.y;
        float cameraOffset = 18.0f;
    
        transform.position = new Vector3(charPosX, charPosY, cameraOffset);
    
     }

    // Update is called once per frame
    void Update()
    {
        cameraFollow();
    }
}
