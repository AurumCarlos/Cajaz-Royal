using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour
{
    
    public float movementSpeed = 1.0f;
    float speedX, speedY;
    
    bool is_walking = false;
    bool is_walking_reversed = false;
    
    Rigidbody2D rb2D;
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
        
        if (speedX < 0) // Gauche
        {
            if (Input.mousePosition[0] - gameObject.transform.position[0] >= 0) (is_walking, is_walking_reversed) = (false, true);
            else (is_walking, is_walking_reversed) = (true, false);
        }
        else if (speedX > 0) // Droite 
        {
            if (Input.mousePosition[0] - gameObject.transform.position[0] >= 0) (is_walking, is_walking_reversed) = (true, false);
            else (is_walking, is_walking_reversed) = (false, true);
        }
        else (is_walking, is_walking_reversed) = (false, false);
        
        if (speedX == 0 && speedY == 0) 
        {
            animator.SetBool("is_walking", false);
            animator.SetBool("is_walking_reversed", false);
        }
        else 
        {
            animator.SetBool("is_walking", is_walking);
            animator.SetBool("is_walking_reversed", is_walking_reversed);
        }
        
        
        rb2D.velocity = new Vector2(speedX, speedY);
    }
}
