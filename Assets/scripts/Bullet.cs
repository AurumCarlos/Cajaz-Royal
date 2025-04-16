using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class bullet : MonoBehaviourPunCallbacks
{

    [Range(0,100)]  [SerializeField] private float speed = -10f;
    [Range(0,10)] [SerializeField] private float lifeTime = 3f;

    public Health PlayerHealth;
    public float Damage;
    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject,lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = transform.up *speed;
    }
    private void OnCollisionEnter2D (Collision2D other)
    {
        Destroy(gameObject);
    }
}
