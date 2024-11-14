using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour
{
	
	[SerializeField] private GameObject BulletPrefab;
	[SerializeField] private Transform firing;
	private float fireRate = 0.5f;
	
    public float movementSpeed = 2.0f;
    float speedX, speedY;
    private bool is_seeing_right;
    Rigidbody2D rb2D;
    public Camera camera;
    public Animator animator;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        speedX = Input.GetAxisRaw("Horizontal") * movementSpeed;
        speedY = Input.GetAxisRaw("Vertical") * movementSpeed;
        
        Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        is_seeing_right = mousePos[0] >= gameObject.transform.position[0];
		        
		
        if (speedX == 0 && speedY == 0) animator.SetBool("is_walking", false);
        else 
        {
            animator.SetBool("is_walking", true);
            
			if (speedX < 0) // Mouvement Gauche
        	{
            	if (is_seeing_right) animator.SetBool("is_reversed", true); // Regarde a droite
            	else animator.SetBool("is_reversed", false);
        	}
        	else if (speedX > 0) // Mouvement Droite 
        	{
            	if (is_seeing_right) animator.SetBool("is_reversed", false);
            	else animator.SetBool("is_reversed", true);
        	}
        }
        
        if (Input.GetMouseButtonDown(0)){
	        Shoot();
	        //fireRate = 0.5f;
        }

		// Rotation while looking
		if (is_seeing_right) gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		else gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);

        rb2D.velocity = new Vector2(speedX, speedY).normalized;

    }
    
    private void Shoot()
    {
	    print("shoot");
	    Instantiate(BulletPrefab,firing.position,firing.rotation);
    }
}
