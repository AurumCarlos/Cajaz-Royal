using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{

    public float HealthP = 0; 
    public float Maxhealth = 100; 
    public Image HealthBar;
    // Start is called before the first frame update
    void Start()
    {
        Maxhealth  = HealthP;
    }

    // Update is called once per frame
    void Update()
    {

        if (HealthBar == null) return;
        HealthBar.fillAmount = Mathf.Clamp(HealthP /Maxhealth,0,1);
    }
}
