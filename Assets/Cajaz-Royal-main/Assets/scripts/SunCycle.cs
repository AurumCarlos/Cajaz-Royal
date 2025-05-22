using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunCycle : MonoBehaviour
{

    [SerializeField] private Light2D sun;
    
    private bool isrising = true; 
    // Start is called before the first frame update
    void Start()
    {
        sun.intensity = 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        
    
        if (sun.intensity <= 0)
        {
            isrising = true;
        }
        else if (sun.intensity >= 1)
        {
            isrising = false;
        }
        float speed = 0.01f;
        if(sun.intensity < 0.2f || sun.intensity > 0.8f)
        {
            speed = 0.005f;
        }
        
        if (isrising)
        {
            sun.intensity += Time.deltaTime * speed;
        }
        else
        {
            sun.intensity -= Time.deltaTime * speed;
        }
    }
}
