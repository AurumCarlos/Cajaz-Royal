using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] private GameObject Creature;
    private float timer = 15;
    private float timer2 = 25;
    private System.Random random = new(0);
  
    // Start is called before the first frame update
    void Start()
    {
        (int, int) p = WalkerGenerator.FindSpawn();
        Instantiate(Creature, new Vector3(p.Item1,p.Item2,0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
            (int, int) p = WalkerGenerator.FindSpawn();
            Instantiate(Creature, new Vector3(p.Item1,p.Item2,0), Quaternion.identity);
            timer = timer2;
            timer2 *= 0.99f;
        }
        timer -= Time.deltaTime;
    }
}
