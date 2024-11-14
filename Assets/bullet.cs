using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{

[Range(1,100)]
[SerializeField] private float speed = -10f;

[Range(1,10)]
[SerializeField] private float lifeTime = 3f;

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
        rb.velocity = transform.up *speed;

    }
        private void OnCollisionEnter2D (Collision2D other){
        if (other.gameObject.CompareTag("enemy")){
            Destroy(other.gameObject);
        }
    }
}
