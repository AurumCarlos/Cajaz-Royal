using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Movements : NetworkBehaviour
{
	
	[SerializeField] private GameObject BulletPrefab;
	[SerializeField] private GameObject ExploPrefab;
	[SerializeField] private Transform firing;
	private float fireRate = 0.5f;
	
	private Vector2 mousePos;
    [Range(1,100)]
    [SerializeField] public float movementSpeed;
    //public float movementSpeed = 5.0f;
    float speedX, speedY;
    private bool is_seeing_right;
    Rigidbody2D rb2D;
    //public Camera camera;
    public Animator animator;
    private string currentanimation;
    private bool is_walking = false;
    private bool is_reversed = false;
    private bool is_shooting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
	    if (!IsOwner) return;
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");
        
        //Vector2 mousePos = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        mousePos = Input.mousePosition;
        is_seeing_right = mousePos[0] >= (gameObject.transform.position[0]+9)*Screen.width/18;
		print((is_seeing_right,mousePos[0],(gameObject.transform.position[0]+9)*1920/9));
		
        if (speedX == 0 && speedY == 0) {
            //animator.SetBool("is_walking", false);
            is_walking = false;
            }
        else 
        {
            //animator.SetBool("is_walking", true);
            is_walking = true;
            
			if (speedX < 0) // Mouvement Gauche
        	{
            	if (is_seeing_right) is_reversed = true;//animator.SetBool("is_reversed", true); // Regarde a droite
                else is_reversed = false; // animator.SetBool("is_reversed", false);
        	}
        	else if (speedX > 0) // Mouvement Droite 
        	{
            	if (is_seeing_right) is_reversed = false; // animator.SetBool("is_reversed", false);
            	else is_reversed = true;// animator.SetBool("is_reversed", true);
        	}
        }
        checkanimation(is_walking,is_reversed);    
		
        if (Input.GetMouseButtonDown(0)){
	        is_shooting = true;
	        //fireRate = 0.5f;
        }if (Input.GetMouseButtonUp(0)){
	        is_shooting = false;
	        //fireRate = 0.5f;
        }
        

        if (is_shooting && fireRate <= 0)
        {
	        Shoot();
	        fireRate = 0.2f;
        }
        fireRate = fireRate -0.01f;



		// Rotation while looking
		if (is_seeing_right) gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		else gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);

        rb2D.velocity = new Vector2(speedX, speedY).normalized*movementSpeed;
    }
    
    private void checkanimation(bool w,bool r){

        if (w==true) 
        {
            if (is_reversed)changeanimation("reversed_walk");
            else changeanimation("Walk");
        }
        else changeanimation("Idle");
    }
	


    private void changeanimation (string animation) 
	{
	if (currentanimation != animation)
		{
		currentanimation = animation;
		animator.Play(animation);
		}
    }

    private void Shoot()
    {
	    Instantiate(BulletPrefab,firing.position,firing.rotation);
	    Instantiate(ExploPrefab,firing.position,firing.rotation);
    }
    
}

