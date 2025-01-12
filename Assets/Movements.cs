using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Movements : MonoBehaviourPunCallbacks
{
	
	[SerializeField] private GameObject BulletPrefab;
	[SerializeField] private GameObject ExploPrefab;
	[SerializeField] private Transform firing;
	private float fireRate = 0.5f;
	

    [Range(1,100)]
    [SerializeField] public float movementSpeed;
    //public float movementSpeed = 5.0f;
    float speedX, speedY;
    private bool is_seeing_right;
    Rigidbody2D rb2D;
    public Camera camera;
    public Animator animator;
    private string currentanimation;
    private bool is_walking = false;
    private bool is_reversed = false;
    private bool is_sprinting = false;
    private SpriteRenderer sr;
    
    // Start is called before the first frame update
    void Start()
    {
	    sr = GetComponent<SpriteRenderer>();
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
	    

        if (Input.GetMouseButtonDown(0)){
	        Shoot();
	        //fireRate = 0.5f;
        }



		// Rotation while looking
		



        rb2D.velocity = new Vector2(speedX, speedY).normalized*(is_sprinting ? (float)(movementSpeed * 2) : movementSpeed);
    }
    


    private void Shoot()
    {
	    Instantiate(BulletPrefab,firing.position,firing.rotation);
	    Instantiate(ExploPrefab,firing.position,firing.rotation);
    }
    
}

