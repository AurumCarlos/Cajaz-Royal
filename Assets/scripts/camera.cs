using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class camera : MonoBehaviourPunCallbacks
{
    
    private Vector2 velocity = Vector2.zero;
    [SerializeField] private GameObject bite;
    Rigidbody2D rb2D;
    private bool is_sprinting = false;


    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    

    void FixedUpdate()
    {
        Vector3 off = transform.position - Camera.main.transform.position;
        Vector3 desiredPosition = bite.transform.position + new Vector3(0, 0, -10);
        Vector3 smoothFollow = Vector3.Lerp(transform.position, desiredPosition, 0.1f);
        transform.position = smoothFollow;
        //transform.Translate(2f *  (!is_sprinting? 0.01f : 0.02f) * new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized);

    }
}
