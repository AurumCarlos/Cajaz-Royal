using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Movements : MonoBehaviourPunCallbacks
{
	
	[SerializeField] private GameObject BulletPrefab;
	private float fireRate = 0.5f;
	private bool is_sprinting;

    [Range(1,100)]
    [SerializeField] public float movementSpeed;
    //public float movementSpeed = 5.0f;
    float speedX, speedY;
    Rigidbody2D rb2D;
    public Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");
        is_sprinting = Input.GetButton("Sprint");

        
        //Vector2 mousePos = Input.mousePosition;
		Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log((mousePos.x,gameObject.transform.position[0],is_seeing_right));	
        //is_seeing_right = mousePos[0]/1000*18/3.840 >= gameObject.transform.position[0] + 9;
	    

        



		// Rotation while looking
		



        //rb2D.velocity = new Vector2(speedX, speedY).normalized*(is_sprinting ? (float)(movementSpeed * 2) : movementSpeed);
    }

    void FixedUpdate()
    {
	    
	    //rb2D.AddForce(mouvement(movementSpeed).normalized*4, ForceMode2D.Force);
	    //rb2D.transform.position = new Vector2(rb2D.transform.position.x + (float)0.05*(Input.GetAxisRaw("Horizontal")),(rb2D.transform.position.y + (float)0.05*Input.GetAxisRaw("Vertical")));
	    //rb2D.transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal")*0.05f, Input.GetAxisRaw("Vertical")*0.05f));
		
	    //rb2D.transform.position = Vector2.SmoothDamp(transform.position,bite.transform.position,ref velocity,0.3f);
    }
    

    
}

